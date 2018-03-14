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
