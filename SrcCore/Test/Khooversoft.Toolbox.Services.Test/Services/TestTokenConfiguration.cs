using Khooversoft.Toolbox.Security;
using Khooversoft.Toolbox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Toolbox.Services.Test.Services
{
    public class TestTokenConfiguration : IServerTokenManagerConfiguration, IClientTokenManagerConfiguration
    {
        private const string _testClientIssuerName = "testClient.issuer@domain.com";
        private const string _testClientSubjectName = "testClient@domain.com";
        private const string _testServerName = "testAuthority@domain.com";

        // ========================================================================================
        // Server configuration
        // ========================================================================================

        public IEnumerable<string> ValidIssuers { get; } = new string[] { _testClientIssuerName };

        public IEnumerable<LocalCertificateKey> TokenAuthorizationRequestCertificateKeys { get; } = new List<LocalCertificateKey>
        {
            new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, Constants.JwtVaultTestCertificateThumbprint, true),
        };

        public TokenAuthorizationConfiguration TokenAuthorization { get; } = new TokenAuthorizationConfiguration(
                new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, Constants.JwtVaultTestCertificateThumbprint, true),
                TimeSpan.FromHours(1),
                _testServerName);

        // ========================================================================================
        // Client configuration
        // ========================================================================================

        public LocalCertificateKey RequestSigningCertificateKey { get; } = new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, Constants.JwtVaultTestCertificateThumbprint, true);

        public string RequestingIssuer { get; } = _testClientIssuerName;

        public Uri AuthorizationUri { get; } = new Uri("HTTP://localhost:8080/v1/token");

        public IEnumerable<LocalCertificateKey> ServerSigningCertificateKeys { get; } = new List<LocalCertificateKey>
        {
            new LocalCertificateKey(StoreLocation.LocalMachine, StoreName.My, Constants.JwtVaultTestCertificateThumbprint, true),
        };

        //public string AuthorizationIssuer { get; } = _testServerName;

        //public string RequestingSubject { get; } = _testClientSubjectName;

        public IEnumerable<string> HmacHeaders { get; } = Enumerable.Empty<string>();

        public TokenKey TokenKey { get; } = new TokenKey(_testServerName, _testClientSubjectName);

        public IHmacConfiguration HmacConfiguration { get; } = new HmacConfiguration();
    }
}
