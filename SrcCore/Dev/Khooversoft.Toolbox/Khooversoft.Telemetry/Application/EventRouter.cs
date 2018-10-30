using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Telemetry
{
    public class EventRouter : MessageRouter<EventData>, IDisposable
    {
        private TrackEventMemoryListener _memoryListener;
        private List<IEventDataWriter> _eventDataWriters = new List<IEventDataWriter>();
        private object _lock = new object();

        public EventRouter(IWorkContext context, bool drainOnDispose = true)
            : base(context, drainOnDispose)
        {
        }

        public new EventRouter Register(string name, Action<EventData, CancellationToken> processMessage)
        {
            base.Register(name, processMessage);
            return this;
        }

        public new EventRouter Remove(string name)
        {
            base.Remove(name);
            return this;
        }

        public TrackEventMemoryListener SetMemoryListener(int size = 10 * 1000)
        {
            Verify.Assert(_memoryListener == null, "Memory listener is already running");

            _memoryListener = new TrackEventMemoryListener(size);
            this.Register("memoryListener", (x, _) => _memoryListener.Post(x));

            return _memoryListener;
        }

        public IEventDataWriter AddLogWriter(string folder)
        {
            IEventDataWriter writer = new LogFileWriter(folder);
            writer.Open();

            this.Register("logWriter", (x, _) => writer.Write(x));

            lock (_lock)
            {
                _eventDataWriters.Add(writer);
            }

            return writer;
        }

        public IEventDataWriter AddLogWriter(string name, IEventDataWriter eventDataWriter)
        {
            Verify.IsNotNull(nameof(eventDataWriter), eventDataWriter);

            this.Register(name, (x, _) => eventDataWriter.Write(x));

            lock (_lock)
            {
                _eventDataWriters.Add(eventDataWriter);
            }

            return eventDataWriter;
        }

        public new void Dispose()
        {
            base.Dispose();

            List<IEventDataWriter> writer = Interlocked.Exchange(ref _eventDataWriters, null);
            writer?.Run(x => x.Dispose());
        }
    }
}
