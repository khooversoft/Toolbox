using System.Collections.Generic;
using System.Linq;

namespace Khooversoft.Security
{
    public class HmacConfiguration : IHmacConfiguration
    {
        private static IEnumerable<string> _standardHeaders = new string[]
        {
            "Content-MD5",
            "Content-Type",
            "API-RequestId",
            "API-Cv",
            "Date"
        };

        public HmacConfiguration(IEnumerable<string> additionalHeaders = null)
        {
            var headers = new List<string>(_standardHeaders);
            if (additionalHeaders != null && additionalHeaders.Count() > 0)
            {
                headers.AddRange(additionalHeaders);
            }

            Headers = headers.Select(x => x.Trim().ToLower()).ToList();
        }

        public IEnumerable<string> Headers { get; }
    }
}
