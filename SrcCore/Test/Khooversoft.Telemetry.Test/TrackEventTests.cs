using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;

namespace Khooversoft.Telemetry.Test
{
    public class TrackEventTests
    {
        [Fact]
        public void TrackEventTest()
        {
            IWorkContext context = WorkContext.Empty;
            var listener = new TrackEventMemoryListener();

            EventRouter router = new EventRouter(context)
                .Register("test", (x, _) => listener.Post(x));

            TrackEventSource source = new TrackEventSource(router, "test");

            listener.Count.Should().Be(0);

            source.Verbose(context, "first message");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            listener.Count.Should().Be(1);

            EventData eventData = listener.Dequeue();
            eventData.EventSourceName.Should().Be("test");
            eventData.EventName.Should().Be("Verbose");
            eventData.TelemetryType.Should().Be(TelemetryType.Verbose);
            eventData.Cv.Should().Be(context.Cv.ToString());
            eventData.Tag.Should().Be(context.Tag.ToString());
        }

        [Fact]
        public void TrackEventLogFileTest()
        {
            IWorkContext context = WorkContext.Empty;
            string tempFolder = Path.Combine(Path.GetTempPath(), "TelemetryTest");
            Directory.CreateDirectory(tempFolder);
            string logFileName;

            using (var logWriter = new LogFileWriter(tempFolder).Open())
            {
                logFileName = logWriter.LogFileName;

                EventRouter router = new EventRouter(context)
                    .Register("test", (x, _) => logWriter.Write(x));

                TrackEventSource source = new TrackEventSource(router, "test");


                source.Verbose(context, "first message");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            var readList = new List<EventData>();
            using (var reader = new LogFileReader(logFileName).Open())
            {
                while (true)
                {
                    IEnumerable<EventData> readItems = reader.Read(100);
                    readItems.Should().NotBeNull();
                    if (!readItems.Any())
                    {
                        break;
                    }

                    readList.AddRange(readItems);
                }
            }

            readList.Count.Should().Be(1);

            EventData eventData = readList[0];
            eventData.EventSourceName.Should().Be("test");
            eventData.EventName.Should().Be("Verbose");
            eventData.TelemetryType.Should().Be(TelemetryType.Verbose);
            eventData.Cv.Should().Be(context.Cv.ToString());
            eventData.Tag.Should().Be(context.Tag.ToString());
        }
    }
}
