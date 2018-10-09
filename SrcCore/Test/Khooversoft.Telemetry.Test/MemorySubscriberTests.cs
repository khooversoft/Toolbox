using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Threading;
using Xunit;

namespace Khooversoft.Telemetry.Test
{
    public class MemorySubscriberTests
    {
        [Fact]
        public void MemoryEventTest()
        {
            IWorkContext context = WorkContext.Empty;
            TelemetryMemoryListener listener = new TelemetryMemoryListener();

            TelemetryRouter router = new TelemetryRouter(context)
                .Register("test", (x, _) => listener.Post(x));

            TelemetrySource source = new TelemetrySource(router, "test");

            listener.Count.Should().Be(0);

            source.Verbose(context, "first message");
            Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            listener.Count.Should().Be(1);

            EventData eventData = listener.Dequeue();
            eventData.EventSourceName.Should().Be("test");
            eventData.EventName.Should().Be("Verbose");
            eventData.TelemetryType.Should().Be(TelemetryType.Verbose);
            eventData.Cv.Should().Be(context.Cv.ToString());
            eventData.Tag.Should().Be(context.Tag.ToString());
        }
    }
}
