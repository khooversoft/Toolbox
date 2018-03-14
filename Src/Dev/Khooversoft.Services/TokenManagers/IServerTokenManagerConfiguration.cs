using Khooversoft.Security;
using System.Collections.Generic;

namespace Khooversoft.Services
{
    public interface IServerTokenManagerConfiguration
    {
        IEnumerable<string> ValidIssuers { get; }

        IEnumerable<LocalCertificateKey> TokenAuthorizationRequestCertificateKeys { get; }

        TokenAuthorizationConfiguration TokenAuthorization { get; }

        IHmacConfiguration HmacConfiguration { get; }
    }
}
