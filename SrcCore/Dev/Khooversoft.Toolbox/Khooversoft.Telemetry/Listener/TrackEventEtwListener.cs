using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using telemetry = Khooversoft.Telemetry;

namespace Khooversoft.Telemetry
{
    public sealed class TrackEventEtwListener : EventSource
    {
        public TrackEventEtwListener(string eventSourceName)
            : base(eventSourceName, EventSourceSettings.ThrowOnEventWriteErrors | EventSourceSettings.EtwSelfDescribingEventFormat)
        {
        }

        [NonEvent]
        public void Post(telemetry.EventData eventDataItem)
        {
            if (eventDataItem == null)
            {
                return;
            }

            var properties = eventDataItem.Properties?.ToDictionary(x => x.Key, x => x.Value?.ToString());
            EventLevel eventLevel = eventDataItem.TelemetryLevel.ConverTo();

            if( eventDataItem.Value != null )
            {
                var logDataValue = new LogDataValue
                {
                    Timestamp = eventDataItem.Timestamp,
                    EventSourceName = eventDataItem.EventSourceName,
                    Message = eventDataItem.Message,
                    Cv = eventDataItem.Cv,
                    Tag = eventDataItem.Tag,
                    Value = (double)eventDataItem.Value,
                    Properties = properties,
                };

                Write(eventDataItem.EventName, new EventSourceOptions { Level = eventLevel }, logDataValue);
                return;
            }

            var logData = new LogData
            {
                Timestamp = eventDataItem.Timestamp,
                EventSourceName = eventDataItem.EventSourceName,
                Message = eventDataItem.Message,
                Cv = eventDataItem.Cv,
                Tag = eventDataItem.Tag,
                Properties = properties,
            };


            Write(eventDataItem.EventName, new EventSourceOptions { Level = eventLevel }, logData);
        }

        [EventData]
        private class LogData
        {
            public DateTimeOffset Timestamp { get; set; }

            public string EventSourceName { get; set; }

            public string Message { get; set; }

            public string Cv { get; set; }

            public string Tag { get; set; }

            public Dictionary<string, string> Properties { get; set; }
        }

        [EventData]
        private class LogDataValue
        {
            public DateTimeOffset Timestamp { get; set; }

            public string EventSourceName { get; set; }

            public string Message { get; set; }

            public string Cv { get; set; }

            public string Tag { get; set; }

            public double Value { get; set; }

            public Dictionary<string, string> Properties { get; set; }
        }
    }
}
