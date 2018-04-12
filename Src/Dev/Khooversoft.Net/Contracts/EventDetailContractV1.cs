// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Khooversoft.Net
{
    [JsonObject]
    public class EventDetailContractV1
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("eventName")]
        public string EventName { get; set; }

        [JsonProperty("eventLevel")]
        public string EventLevel { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("description")]
        public string Message { get; set; }

        [JsonProperty("properties")]
        public IDictionary<string, string> Properties { get; set; }
    }
}
