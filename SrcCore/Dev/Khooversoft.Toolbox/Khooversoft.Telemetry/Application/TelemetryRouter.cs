using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Telemetry
{
    public class TelemetryRouter
    {
        private readonly RingQueue<EventData> _queue = new RingQueue<EventData>(1000);
        private readonly Dictionary<string, Action<EventData, CancellationToken>> _actions = new Dictionary<string, Action<EventData, CancellationToken>>();
        private readonly IWorkContext _workContext;
        private int _processLock = 0;
        private object _lock = new object();
        private readonly Timer _timer;
        private List<Action<EventData, CancellationToken>> _cacheList;

        public TelemetryRouter(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            _workContext = context;
            _timer = new Timer(ProcessQueue, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
        }

        public TelemetryRouter Register(string name, Action<EventData, CancellationToken> processEventData)
        {
            Verify.IsNotEmpty(nameof(name), name);
            Verify.IsNotNull(nameof(processEventData), processEventData);

            lock (_lock)
            {
                _actions[name] = processEventData;
                _cacheList = null;
            }

            return this;
        }

        public TelemetryRouter Remove(string name)
        {
            Verify.IsNotEmpty(nameof(name), name);

            lock (_lock)
            {
                _actions.Remove(name);
                _cacheList = null;
            }

            return this;
        }

        public void Post(EventData eventData)
        {
            Verify.IsNotNull(nameof(eventData), eventData);

            _queue.Enqueue(eventData);
        }

        private void ProcessQueue(object arg)
        {
            int currentLockValue = Interlocked.CompareExchange(ref _processLock, 1, 0);
            if (currentLockValue == 1)
            {
                return;
            }

            lock (_lock)
            {
                _cacheList = _cacheList ?? new List<Action<EventData, CancellationToken>>(_actions.Values);
            }

            try
            {
                while (true)
                {
                    (bool Success, EventData value) = _queue.TryDequeue();
                    if (!Success)
                    {
                        break;
                    }

                    foreach(var item in _cacheList)
                    {
                        item(value, _workContext.CancellationToken);
                    }
                }
            }
            finally
            {
                Interlocked.CompareExchange(ref _processLock, 0, 1);
            }
        }
    }
}
