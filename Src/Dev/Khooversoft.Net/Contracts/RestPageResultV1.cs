// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Khooversoft.Net
{
    [JsonObject]
    public class RestPageResultV1<T>
    {
        [JsonProperty("items")]
        public IList<T> Items { get; set; }

        [JsonProperty("continueIndexUri")]
        public string ContinueIndexUri { get; set; }
    }
}
