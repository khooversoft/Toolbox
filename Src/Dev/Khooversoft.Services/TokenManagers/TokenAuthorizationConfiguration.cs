using Khooversoft.Security;
using Khooversoft.Toolbox;
using System;

namespace Khooversoft.Services
{
    public class TokenAuthorizationConfiguration
    {
        public TokenAuthorizationConfiguration(LocalCertificateKey signingCertificateKey, TimeSpan goodFor, string issuer)
        {
            Verify.IsNotNull(nameof(signingCertificateKey), signingCertificateKey);
            Verify.IsNotNull(nameof(goodFor), goodFor);
            Verify.IsNotEmpty(nameof(issuer), issuer);

            AuthorizationSigningCertificateKey = signingCertificateKey;
            GoodFor = goodFor;
            Issuer = issuer;
        }

        public TokenAuthorizationConfiguration(LocalCertificateKey localCertificateKey, TimeSpan goodFor, string issuer, string audience)
            : this(localCertificateKey, goodFor, issuer)
        {
            Verify.IsNotEmpty(nameof(audience), audience);

            Audience = audience;
        }

        public LocalCertificateKey AuthorizationSigningCertificateKey { get; }

        public TimeSpan GoodFor { get; }

        public string Issuer { get; }

        public string Audience { get; }
    }
}
