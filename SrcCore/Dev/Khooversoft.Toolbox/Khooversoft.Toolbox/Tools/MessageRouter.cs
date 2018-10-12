using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Toolbox
{
    public class MessageRouter<T> : IDisposable
    {
        private const int False = 0;
        private const int True = 1;

        private const string _shutdownMessage = "Message router is shut down";
        private readonly RingQueue<T> _queue = new RingQueue<T>(1000);
        private readonly Dictionary<string, Action<T, CancellationToken>> _actions = new Dictionary<string, Action<T, CancellationToken>>();
        private readonly IWorkContext _workContext;
        private int _processLock = 0;
        private readonly object _lock = new object();
        private readonly object _processingLock = new object();
        private Timer _timer;
        private List<Action<T, CancellationToken>> _cacheList;
        private int _shutdown = False;

        public MessageRouter(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            _workContext = context;
            _timer = new Timer(ProcessQueue, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
        }

        public MessageRouter<T> Register(string name, Action<T, CancellationToken> processMessage)
        {
            Verify.IsNotEmpty(nameof(name), name);
            Verify.IsNotNull(nameof(processMessage), processMessage);
            Verify.Assert<InvalidOperationException>(_shutdown == False, _shutdownMessage);

            lock (_lock)
            {
                _actions[name] = processMessage;
                _cacheList = null;
            }

            return this;
        }

        public MessageRouter<T> Remove(string name)
        {
            Verify.IsNotEmpty(nameof(name), name);
            Verify.Assert<InvalidOperationException>(_shutdown == False, _shutdownMessage);

            lock (_lock)
            {
                _actions.Remove(name);
                _cacheList = null;
            }

            return this;
        }

        public void Post(T message)
        {
            Verify.IsNotNull(nameof(message), message);
            Verify.Assert<InvalidOperationException>(_shutdown == False, _shutdownMessage);

            _queue.Enqueue(message);
        }

        public void Close(bool drain)
        {
            int shutdown = Interlocked.CompareExchange(ref _shutdown, True, False);
            if (shutdown == True)
            {
                return;
            }

            Timer t = Interlocked.Exchange(ref _timer, null);
            _timer?.Dispose();

            if (drain)
            {
                lock (_processingLock)
                {
                    SendMessage(_actions.Values);
                }
            }
        }

        public void Dispose()
        {
            Close(false);
        }

        private void ProcessQueue(object arg)
        {
            if (_shutdown == True)
            {
                return;
            }

            int currentLockValue = Interlocked.CompareExchange(ref _processLock, 1, 0);
            if (currentLockValue == 1)
            {
                return;
            }

            lock (_lock)
            {
                _cacheList = _cacheList ?? new List<Action<T, CancellationToken>>(_actions.Values);
            }

            try
            {
                lock (_processingLock)
                {
                    SendMessage(_cacheList);
                }
            }
            finally
            {
                Interlocked.CompareExchange(ref _processLock, 0, 1);
            }
        }

        private void SendMessage(IEnumerable<Action<T, CancellationToken>> actionList)
        {
            while (true)
            {
                (bool Success, T value) = _queue.TryDequeue();
                if (!Success)
                {
                    return;
                }

                foreach (var item in actionList)
                {
                    item(value, _workContext.CancellationToken);
                }
            }
        }
    }
}
