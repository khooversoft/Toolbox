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
                EventLevel = self.TelemetryType.ToString(),
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
                telemetryType: (TelemetryType)Enum.Parse(typeof(TelemetryType), record.EventLevel),
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

        //public static void Test()
        //{
        //    var record = new EventDataRecord
        //    {
        //        Date = DateTime.Now.ToString("o"),
        //        Version = 1,
        //        EventName = "eventName",
        //        EventSourceName = "eventSourceName",
        //        EventLevel = "eventLevel",
        //    };

        //    var output = new OutputBuffer();
        //    var writer = new CompactBinaryWriter<OutputBuffer>(output);

        //    // The first calls to Serialize.To and Deserialize<T>.From can take
        //    // a relatively long time because they generate the de/serializer
        //    // for a given type and protocol.
        //    Bond.Serialize.To(writer, record);

        //    var input = new InputBuffer(output.Data);
        //    var reader = new CompactBinaryReader<InputBuffer>(input);

        //    var dst = Deserialize<EventDataRecord>.From(reader);
        //}
    }
}
