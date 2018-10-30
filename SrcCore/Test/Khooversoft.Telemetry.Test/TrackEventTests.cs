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

            using (EventRouter router = new EventRouter(context).Register("test", (x, _) => listener.Post(x)))
            {
                TrackEventSource source = new TrackEventSource(router, "test");

                listener.Count.Should().Be(0);

                source.Verbose(context, "first message");
                Thread.Sleep(TimeSpan.FromSeconds(1));
                listener.Count.Should().Be(1);

                EventData eventData = listener.Dequeue();
                eventData.EventSourceName.Should().Be("test");
                eventData.EventName.Should().Be("Verbose");
                eventData.TelemetryLevel.Should().Be(TelemetryLevel.Verbose);
                eventData.Cv.Should().Be(context.Cv.ToString());
                eventData.Tag.Should().Be(context.Tag.ToString());
            }
        }

        [Fact]
        public void TrackEventDrainFalseTest()
        {
            TrackEventSource source;

            IWorkContext context = WorkContext.Empty;
            var listener = new TrackEventMemoryListener();

            using (EventRouter router = new EventRouter(context, drainOnDispose: false).Register("test", (x, _) => listener.Post(x)))
            {
                source = new TrackEventSource(router, "test");

                listener.Count.Should().Be(0);

                source.Verbose(context, "first message");
            }

            listener.Count.Should().Be(0);
        }

        [Fact]
        public void TrackEventLogFileTest()
        {
            IWorkContext context = WorkContext.Empty;
            string tempFolder = Path.Combine(Path.GetTempPath(), "TelemetryTest");
            Directory.CreateDirectory(tempFolder);
            string logFileName;

            using (var logWriter = new LogFileWriter(tempFolder).Open())
            using (EventRouter router = new EventRouter(context).Register("test", (x, _) => logWriter.Write(x)))
            {
                logFileName = logWriter.LogFileName;

                TrackEventSource source = new TrackEventSource(router, "test");

                source.Verbose(context, "first message");
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
            eventData.TelemetryLevel.Should().Be(TelemetryLevel.Verbose);
            eventData.Cv.Should().Be(context.Cv.ToString());
            eventData.Tag.Should().Be(context.Tag.ToString());
        }

        [Fact]
        public void TrackEventLogFileWithManagementTest()
        {
            IWorkContext context = WorkContext.Empty;
            string tempFolder = Path.Combine(Path.GetTempPath(), "TelemetryTest");
            Directory.CreateDirectory(tempFolder);
            TrackEventMemoryListener listener;

            using (EventRouter router = new EventRouter(context))
            {
                listener = router.SetMemoryListener();
                TrackEventSource source = new TrackEventSource(router, "test");

                source.Verbose(context, "first message");
            }

            listener.Count.Should().Be(1);

            EventData eventData = listener.Dequeue();
            eventData.EventSourceName.Should().Be("test");
            eventData.EventName.Should().Be("Verbose");
            eventData.TelemetryLevel.Should().Be(TelemetryLevel.Verbose);
            eventData.Cv.Should().Be(context.Cv.ToString());
            eventData.Tag.Should().Be(context.Tag.ToString());
        }

        [Fact]
        public void MultipleTrackEventLogFileTest()
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

                Enumerable.Range(0, 100).Run(x =>
                {
                    source.Verbose(context, $"{x}_verbose message");
                    source.Info(context, $"{x}_info message");
                    source.Critical(context, $"{x}_critical message");
                });

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            var readList = new List<EventData>();
            using (var reader = new LogFileReader(logFileName).Open())
            {
                while (true)
                {
                    IEnumerable<EventData> readItems = reader.Read(500);
                    readItems.Should().NotBeNull();
                    if (!readItems.Any())
                    {
                        break;
                    }

                    readList.AddRange(readItems);
                }
            }

            readList.Count.Should().Be(300);

            IEnumerator<EventData> dataEnumerable = readList.GetEnumerator();
            Enumerable.Range(0, 100).Run(x =>
            {
                EventData ev;

                dataEnumerable.MoveNext().Should().BeTrue();
                ev = dataEnumerable.Current;
                ev.EventSourceName.Should().Be("test");
                ev.EventName.Should().Be("Verbose");
                ev.TelemetryLevel.Should().Be(TelemetryLevel.Verbose);
                ev.Cv.Should().Be(context.Cv.ToString());
                ev.Tag.Should().Be(context.Tag.ToString());
                ev.Message.Should().Be($"{x}_verbose message");

                dataEnumerable.MoveNext().Should().BeTrue();
                ev = dataEnumerable.Current;
                ev.EventSourceName.Should().Be("test");
                ev.EventName.Should().Be("Info");
                ev.TelemetryLevel.Should().Be(TelemetryLevel.Informational);
                ev.Cv.Should().Be(context.Cv.ToString());
                ev.Tag.Should().Be(context.Tag.ToString());
                ev.Message.Should().Be($"{x}_info message");

                dataEnumerable.MoveNext().Should().BeTrue();
                ev = dataEnumerable.Current;
                ev.EventSourceName.Should().Be("test");
                ev.EventName.Should().Be("Critical");
                ev.TelemetryLevel.Should().Be(TelemetryLevel.Critical);
                ev.Cv.Should().Be(context.Cv.ToString());
                ev.Tag.Should().Be(context.Tag.ToString());
                ev.Message.Should().Be($"{x}_critical message");
            });
        }

        [Fact]
        public void MultipleTrackEventLogFileWithManagementTest()
        {
            IWorkContext context = WorkContext.Empty;
            string tempFolder = Path.Combine(Path.GetTempPath(), "TelemetryTest");
            Directory.CreateDirectory(tempFolder);
            string logFileName;

            using (EventRouter router = new EventRouter(context))
            {
                IEventDataWriter writer = router.AddLogWriter(tempFolder);
                Verify.IsNotNull(nameof(writer), writer);
                logFileName = writer.LogFileName;

                TrackEventSource source = new TrackEventSource(router, "test");

                Enumerable.Range(0, 100).Run(x =>
                {
                    source.Verbose(context, $"{x}_verbose message");
                    source.Info(context, $"{x}_info message");
                    source.Critical(context, $"{x}_critical message");
                });
            }

            var readList = new List<EventData>();
            using (var reader = new LogFileReader(logFileName).Open())
            {
                IEnumerable<EventData> readItems = reader.Read(500);
                readItems.Should().NotBeNull();
                readItems.Any().Should().BeTrue();

                readList.AddRange(readItems);
            }

            readList.Count.Should().Be(300);

            IEnumerator<EventData> dataEnumerable = readList.GetEnumerator();
            Enumerable.Range(0, 100).Run(x =>
            {
                EventData ev;

                dataEnumerable.MoveNext().Should().BeTrue();
                ev = dataEnumerable.Current;
                ev.EventSourceName.Should().Be("test");
                ev.EventName.Should().Be("Verbose");
                ev.TelemetryLevel.Should().Be(TelemetryLevel.Verbose);
                ev.Cv.Should().Be(context.Cv.ToString());
                ev.Tag.Should().Be(context.Tag.ToString());
                ev.Message.Should().Be($"{x}_verbose message");

                dataEnumerable.MoveNext().Should().BeTrue();
                ev = dataEnumerable.Current;
                ev.EventSourceName.Should().Be("test");
                ev.EventName.Should().Be("Info");
                ev.TelemetryLevel.Should().Be(TelemetryLevel.Informational);
                ev.Cv.Should().Be(context.Cv.ToString());
                ev.Tag.Should().Be(context.Tag.ToString());
                ev.Message.Should().Be($"{x}_info message");

                dataEnumerable.MoveNext().Should().BeTrue();
                ev = dataEnumerable.Current;
                ev.EventSourceName.Should().Be("test");
                ev.EventName.Should().Be("Critical");
                ev.TelemetryLevel.Should().Be(TelemetryLevel.Critical);
                ev.Cv.Should().Be(context.Cv.ToString());
                ev.Tag.Should().Be(context.Tag.ToString());
                ev.Message.Should().Be($"{x}_critical message");
            });
        }
    }
}
