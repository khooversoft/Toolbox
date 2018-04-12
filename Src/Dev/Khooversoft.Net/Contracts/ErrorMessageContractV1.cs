// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Khooversoft.Net
{
    [JsonObject]
    public class ErrorMessageContractV1
    {
        [JsonProperty("httpStatus")]
        public int HttpStatus { get; set; }

        [JsonProperty("requestUrl")]
        public string RequestUrl { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("exceptionType")]
        public string ExceptionType { get; set; }

        [JsonProperty("detailMessage")]
        public string DetailMessage { get; set; }

        [JsonProperty("cv")]
        public string Cv { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("eventDetails")]
        public IEnumerable<EventDetailContractV1> EventDetails { get; set; }
    }
}
