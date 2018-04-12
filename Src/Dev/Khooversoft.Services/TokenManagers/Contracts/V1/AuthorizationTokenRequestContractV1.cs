// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;

namespace Khooversoft.Services
{
    [JsonObject]
    public class AuthorizationTokenRequestContractV1
    {
        [JsonProperty("requestToken")]
        public string RequestToken { get; set; }
    }
}
