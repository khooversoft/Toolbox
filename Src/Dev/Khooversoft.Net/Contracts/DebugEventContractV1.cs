// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Khooversoft.Net
{
    [JsonObject]
    public class DebugEventContractV1
    {
        [JsonProperty("eventData")]
        public IList<EventDetailContractV1> EventData { get; set; }
    }
}
