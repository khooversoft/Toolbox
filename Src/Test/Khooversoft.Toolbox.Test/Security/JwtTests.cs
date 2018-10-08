using FluentAssertions;
using Khooversoft.Toolbox.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Khooversoft.Toolbox.Test.Security
{
    [Trait("Category", "JWT")]
    public class JwtTests
    {
        private static readonly IWorkContext _workContext = WorkContext.Empty;

        private static readonly LocalCertificate _jwtVaultTest =
            new LocalCertificate(StoreLocation.LocalMachine, StoreName.My, "CB965B095107F792022114ED67A8B2E8C4DFBCA8", true);

        private static readonly LocalCertificate _vaultData =
            new LocalCertificate(StoreLocation.LocalMachine, StoreName.My, "5B76606DD3076D73825CA2FA04C7D285AEBC7C73", true);

        [Fact]
        public void JwtSecurityTokenBuilderTest()
        {
            const string userId = "user@domain.com";
            const string emailText = "Email";
            const string emailId = "testemail@domain.com";

            string token = new JwtTokenBuilder()
                .SetAudience(new Uri("http://localhost/audience").ToString())
                .SetIssuer(new Uri("http://localhost/Issuer").ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim(emailText, emailId))
                .AddSubject(userId)
                .SetCertificate(_jwtVaultTest.GetCertificate(_workContext))
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_jwtVaultTest.GetCertificate(_workContext))
                .Build()
                .Parse(_workContext, token);

            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(_jwtVaultTest.LocalCertificateKey.Thumbprint);
            tokenDetails.JwtSecurityToken.Subject.Should().Be(userId);
            tokenDetails.JwtSecurityToken.Claims.Any(x => x.Type == emailText && x.Value == emailId).Should().BeTrue();
        }

        [Fact]
        public void JwtSecurityTokenAudienceTest()
        {
            var audience = new Uri("http://localhost/audience");

            string token = new JwtTokenBuilder()
                .SetAudience(audience.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(_jwtVaultTest.GetCertificate(_workContext))
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_jwtVaultTest.GetCertificate(_workContext))
                .AddValidAudience(audience.ToString())
                .Build()
                .Parse(_workContext, token);

            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(_jwtVaultTest.LocalCertificateKey.Thumbprint);
        }

        [Fact]
        public void JwtSecurityTokenIssuerTest()
        {
            var issuer = new Uri("http://localhost/Issuer");

            string token = new JwtTokenBuilder()
                .SetIssuer(issuer.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(_jwtVaultTest.GetCertificate(_workContext))
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_jwtVaultTest.GetCertificate(_workContext))
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(_workContext, token);

            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(_jwtVaultTest.LocalCertificateKey.Thumbprint);
        }

        [Fact]
        public void JwtSecurityFailureTest()
        {
            var issuer = new Uri("http://localhost/Issuer");

            string token = new JwtTokenBuilder()
                .SetIssuer(issuer.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(_jwtVaultTest.GetCertificate(_workContext))
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_vaultData.GetCertificate(_workContext))
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(_workContext, token);

            // Failure
            tokenDetails.Should().BeNull();

            tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_jwtVaultTest.GetCertificate(_workContext))
                .AddCertificate(_vaultData.GetCertificate(_workContext))
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(_workContext, token);

            tokenDetails.Should().NotBeNull();
            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(_jwtVaultTest.LocalCertificateKey.Thumbprint);
        }

        [Fact]
        public void JwtSecuritySignatureFailureTest()
        {
            var issuer = new Uri("http://localhost/Issuer");

            string token = new JwtTokenBuilder()
                .SetIssuer(issuer.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(_jwtVaultTest.GetCertificate(_workContext))
                .Build();

            token.Should().NotBeNullOrEmpty();

            token = token.Remove(3, 2);

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(_jwtVaultTest.GetCertificate(_workContext))
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(_workContext, token);

            tokenDetails.Should().BeNull();
        }
    }
}
