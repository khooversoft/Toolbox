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
