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
