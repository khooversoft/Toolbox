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
            DateTime timestamp,
            int version,
            string eventName,
            string eventSourceName,
            EventLevel eventLevel,
            string tag,
            string cv,
            IEnumerable<KeyValuePair<string, object>> properties
            )
        {
            Verify.IsNotEmpty(nameof(eventName), eventName);
            Verify.IsNotNull(nameof(eventSourceName), eventSourceName);

            Timestamp = timestamp;
            Version = version;
            EventName = eventName;
            EventSourceName = eventSourceName;
            EventLevel = eventLevel;
            Tag = tag;
            Cv = cv;
            Properties = properties?.ToList();
        }

        public DateTimeOffset Timestamp { get; }

        public int Version { get; }

        public string EventName { get; }

        public string EventSourceName { get; }

        public EventLevel EventLevel { get; }

        public string Tag { get; }

        public string Cv { get; }

        public IReadOnlyList<KeyValuePair<string, object>> Properties { get; }
    }
}
