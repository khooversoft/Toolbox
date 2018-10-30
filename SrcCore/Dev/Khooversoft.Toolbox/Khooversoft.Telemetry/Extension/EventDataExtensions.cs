// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Telemetry.Schemas;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Khooversoft.Telemetry
{
    public static class EventDataExtensions
    {
        public static EventDataRecord ConvertTo(this EventData self)
        {
            var record = new EventDataRecord
            {
                Timestamp = self.Timestamp.ToString("o"),
                EventName = self.EventName,
                EventSourceName = self.EventSourceName,
                EventLevel = self.TelemetryLevel.ToString(),
                Message = self.Message,
                Tag = self.Tag,
                Cv = self.Cv,
                Value = self.Value,
            };

            if (self.Properties != null)
            {
                int propertyNumber = -1;
                foreach (var item in self.Properties)
                {
                    propertyNumber++;

                    switch (item.Value)
                    {
                        case int x:
                            record.IntProperties = record.IntProperties ?? new List<PropertyDataInt>();
                            record.IntProperties.Add(new PropertyDataInt { PropertyName = item.Key, Value = x });
                            break;

                        case long x:
                            record.LongProperties = record.LongProperties ?? new List<PropertyDataLong>();
                            record.LongProperties.Add(new PropertyDataLong { PropertyName = item.Key, Value = x });
                            break;

                        case DateTime x:
                            record.DateProperties = record.DateProperties ?? new List<PropertyDataDate>();
                            record.DateProperties.Add(new PropertyDataDate { PropertyName = item.Key, Value = x.ToString("o") });
                            break;

                        case string x:
                            record.StringProperties = record.StringProperties ?? new List<PropertyDataString>();
                            record.StringProperties.Add(new PropertyDataString { PropertyName = item.Key, Value = x });
                            break;

                        default:
                            throw new InvalidOperationException($"Type={item.Value.GetType().Name} is not support");
                    }
                }
            }

            return record;
        }

        public static EventData ConverTo(this EventDataRecord record)
        {
            List<KeyValuePair<string, object>> properties = new List<KeyValuePair<string, object>>();

            process(record.IntProperties?.Count, index => new KeyValuePair<string, object>(record.IntProperties[index].PropertyName, record.IntProperties[index].Value));
            process(record.LongProperties?.Count, index => new KeyValuePair<string, object>(record.LongProperties[index].PropertyName, record.LongProperties[index].Value));
            process(record.StringProperties?.Count, index => new KeyValuePair<string, object>(record.StringProperties[index].PropertyName, record.StringProperties[index].Value));

            process(record.DateProperties?.Count, index =>
            {
                DateTime dateTime = DateTime.Parse(record.DateProperties[index].Value, null, DateTimeStyles.RoundtripKind);
                return new KeyValuePair<string, object>(record.DateProperties[index].PropertyName, dateTime);
            });

            return new EventData(
                timestamp: DateTime.Parse(record.Timestamp, null, DateTimeStyles.RoundtripKind),
                eventName: record.EventName,
                eventSourceName: record.EventSourceName,
                telemetryLevel: (TelemetryLevel)Enum.Parse(typeof(TelemetryLevel), record.EventLevel),
                message: record.Message,
                tag: record.Tag,
                cv: record.Cv,
                value: record.Value,
                properties: properties
                );

            // Local method for processing properties
            void process(int? count, Func<int, KeyValuePair<string, object>> getData)
            {
                if (count == null)
                {
                    return;
                }

                for (int i = 0; i < count; i++)
                {
                    properties.Add(getData(i));
                }
            };
        }

        public static EventData ConvertTo(this EventWrittenEventArgs args)
        {
            var properties = args.PayloadNames
                .Zip(args.Payload, (key, v) => new KeyValuePair<string, object>(key, v))
                .ToList();

            DateTimeOffset timestamp = DateTimeOffset.UtcNow;
            string eventSourceName = args.EventSource.Name;
            string message = null;
            string cv = null;
            string tag = null;
            double? value = null;

            foreach(KeyValuePair<string, object> property in properties)
            {
                switch( property.Key)
                {
                    case "Timestamp":
                        timestamp = (DateTimeOffset)property.Value;
                        break;

                    case "EventSourceName":
                        eventSourceName = (string)property.Value;
                        break;

                    case "Message":
                        message = (string)property.Value;
                        break;

                    case "Cv":
                        cv = (string)property.Value;
                        break;

                    case "Tag":
                        tag = (string)property.Value;
                        break;

                    case "Value":
                        value = (double)property.Value;
                        break;
                }
            }

            return new EventData(
                timestamp: timestamp,
                eventSourceName: eventSourceName,
                eventName: args.EventName,
                telemetryLevel: args.Level.ConvertTo(),
                message: message,
                cv: cv,
                tag: tag,
                value: value,
                properties: properties
                );
        }

        public static TelemetryLevel ConvertTo(this EventLevel eventLevel)
        {
            switch (eventLevel)
            {
                case EventLevel.Verbose:
                    return TelemetryLevel.Verbose;

                case EventLevel.Informational:
                    return TelemetryLevel.Informational;

                case EventLevel.Warning:
                    return TelemetryLevel.Warning;

                case EventLevel.Error:
                    return TelemetryLevel.Error;

                case EventLevel.Critical:
                    return TelemetryLevel.Critical;

                default:
                    return TelemetryLevel.Verbose;
            }
        }

        public static EventLevel ConverTo(this TelemetryLevel telemetryType)
        {
            switch (telemetryType)
            {
                case TelemetryLevel.Verbose:
                    return EventLevel.Verbose;

                case TelemetryLevel.Informational:
                    return EventLevel.Informational;

                case TelemetryLevel.Warning:
                    return EventLevel.Warning;

                case TelemetryLevel.Error:
                    return EventLevel.Error;

                case TelemetryLevel.Critical:
                    return EventLevel.Critical;

                default:
                    return EventLevel.Verbose;
            }

        }
    }
}
