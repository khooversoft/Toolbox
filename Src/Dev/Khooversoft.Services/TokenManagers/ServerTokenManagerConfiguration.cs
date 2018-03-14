using Khooversoft.Security;
using System.Collections.Generic;

namespace Khooversoft.Services
{
    public class ServerTokenManagerConfiguration : IServerTokenManagerConfiguration
    {
        public ServerTokenManagerConfiguration()
        {
        }

        public IEnumerable<string> ValidIssuers { get; set; }

        public IEnumerable<LocalCertificateKey> TokenAuthorizationRequestCertificateKeys { get; set; }

        public TokenAuthorizationConfiguration TokenAuthorization { get; set; }

        public IHmacConfiguration HmacConfiguration { get; set; }
    }
}
