using Khooversoft.Toolbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Telemetry
{
    public class TrackEventMemoryListener : RingQueue<EventData>
    {
        public TrackEventMemoryListener(int size)
            : base(size)
        {
        }

        public TrackEventMemoryListener()
            : this(1000)
        {
        }

        public void Post(EventData eventData)
        {
            Verify.IsNotNull(nameof(eventData), eventData);

            Enqueue(eventData);
        }
    }
}
