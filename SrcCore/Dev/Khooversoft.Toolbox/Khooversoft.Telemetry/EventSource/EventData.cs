// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Telemetry
{
    public class EventData
    {
        public EventData(
            string eventSourceName,
            string eventName,
            TelemetryType telemetryType,
            string cv,
            string tag,
            IEnumerable<KeyValuePair<string, object>> properties
            )
            : this(DateTime.UtcNow, eventSourceName, eventName, telemetryType, cv, tag, null, properties)
        {
        }

        public EventData(
            string eventSourceName,
            string eventName,
            TelemetryType telemetryType,
            string cv,
            string tag,
            double value,
            IEnumerable<KeyValuePair<string, object>> properties
            )
            : this(DateTime.UtcNow, eventSourceName, eventName, telemetryType, cv, tag, value, properties)
        {
        }

        public EventData(
            DateTime timestamp,
            string eventSourceName,
            string eventName,
            TelemetryType telemetryType,
            string cv,
            string tag,
            double? value,
            IEnumerable<KeyValuePair<string, object>> properties
            )
        {
            Verify.IsNotEmpty(nameof(eventName), eventName);
            Verify.IsNotNull(nameof(eventSourceName), eventSourceName);

            Timestamp = timestamp;
            EventSourceName = eventSourceName;
            EventName = eventName;
            TelemetryType = telemetryType;
            Cv = cv;
            Tag = tag;
            Value = value;
            Properties = properties?.ToList();
        }

        public DateTimeOffset Timestamp { get; }

        public string EventSourceName { get; }

        public string EventName { get; }

        public TelemetryType TelemetryType { get; }

        public string Cv { get; }

        public string Tag { get; }

        public double? Value { get; }

        public IReadOnlyList<KeyValuePair<string, object>> Properties { get; }
    }
}
