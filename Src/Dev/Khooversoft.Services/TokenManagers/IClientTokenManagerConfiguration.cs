using Khooversoft.Security;
using System;
using System.Collections.Generic;

namespace Khooversoft.Services
{
    public interface IClientTokenManagerConfiguration
    {
        LocalCertificateKey RequestSigningCertificateKey { get; }

        string RequestingIssuer { get; }

        Uri AuthorizationUri { get; }

        IEnumerable<LocalCertificateKey> ServerSigningCertificateKeys { get; }

        TokenKey TokenKey { get; }

        IHmacConfiguration HmacConfiguration { get; }
    }
}
