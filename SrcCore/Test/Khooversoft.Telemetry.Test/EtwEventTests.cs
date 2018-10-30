using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;
using System.Threading;
using Xunit;

namespace Khooversoft.Telemetry.Test
{
    public class EtwEventTests
    {
        [Fact]
        public void TrackEventTest()
        {
            IWorkContext context = WorkContext.Empty;
            var listener = new TrackEventEtwListener("EtwEventTests.TrackEventTest");

            using (var eventListener = new EventSourceListener())
            {
                eventListener.EnableEvents(listener, EventLevel.LogAlways);

                using (EventRouter router = new EventRouter(context).Register("test", (x, _) => listener.Post(x)))
                {
                    TrackEventSource source = new TrackEventSource(router, "test");

                    source.Verbose(context, "first message");
                    Thread.Sleep(TimeSpan.FromSeconds(1));

                    eventListener.EventDataItems.Count.Should().Be(1);
                    EventData eventData = eventListener.EventDataItems[0];

                    eventData.EventSourceName.Should().Be("test");
                    eventData.EventName.Should().Be("Verbose");
                    eventData.TelemetryLevel.Should().Be(TelemetryLevel.Verbose);
                    eventData.Cv.Should().Be(context.Cv.ToString());
                    eventData.Tag.Should().Be(context.Tag.ToString());
                }
            }
        }

        private class EventSourceListener : EventListener
        {
            protected override void OnEventWritten(EventWrittenEventArgs eventData)
            {
                base.OnEventWritten(eventData);

                EventDataItems.Add(eventData.ConvertTo());
            }

            public List<EventData> EventDataItems { get; } = new List<EventData>();
        }
    }
}
