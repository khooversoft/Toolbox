using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Khooversoft.Telemetry
{
    public class EventRouter : MessageRouter<EventData>
    {
        public EventRouter(IWorkContext context)
            : base(context)
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
    }
}
