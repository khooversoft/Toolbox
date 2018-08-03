using FluentAssertions;
using Khooversoft.Toolbox.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Khooversoft.Toolbox.Test.Services
{
    [Trait("Category", "JWT")]
    public class JwtTests
    {
        private static IWorkContext _workContext = WorkContext.Empty;

        [Fact]
        public void JwtSecurityTokenBuilderTest()
        {
            const string userId = "user@domain.com";
            const string emailText = "Email";
            const string emailId = "testemail@domain.com";

            var localCertificate = new LocalCertificate(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true);

            string token = new JwtTokenBuilder()
                .SetAudience(new Uri("http://localhost/audience").ToString())
                .SetIssuer(new Uri("http://localhost/Issuer").ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim(emailText, emailId))
                .AddSubject(userId)
                .SetCertificate(localCertificate.GetCertificate(_workContext))
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(localCertificate.GetCertificate(_workContext))
                .Build()
                .Parse(_workContext, token);

            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(localCertificate.LocalCertificateKey.Thumbprint);
            tokenDetails.JwtSecurityToken.Subject.Should().Be(userId);
            tokenDetails.JwtSecurityToken.Claims.Any(x => x.Type == emailText && x.Value == emailId).Should().BeTrue();
        }

        [Fact]
        public void JwtSecurityTokenAudienceTest()
        {
            var audience = new Uri("http://localhost/audience");
            var localCertificate = new LocalCertificate(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true);

            string token = new JwtTokenBuilder()
                .SetAudience(audience.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(localCertificate.GetCertificate(_workContext))
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(localCertificate.GetCertificate(_workContext))
                .AddValidAudience(audience.ToString())
                .Build()
                .Parse(_workContext, token);

            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(localCertificate.LocalCertificateKey.Thumbprint);
        }

        [Fact]
        public void JwtSecurityTokenIssuerTest()
        {
            var issuer = new Uri("http://localhost/Issuer");
            var localCertificate = new LocalCertificate(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true);

            string token = new JwtTokenBuilder()
                .SetIssuer(issuer.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(localCertificate.GetCertificate(_workContext))
                .Build();

            token.Should().NotBeNullOrEmpty();

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(localCertificate.GetCertificate(_workContext))
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(_workContext, token);

            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(localCertificate.LocalCertificateKey.Thumbprint);
        }

        [Fact]
        public void JwtSecurityFailureTest()
        {
            var issuer = new Uri("http://localhost/Issuer");
            var localCertificate = new LocalCertificate(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true);

            string token = new JwtTokenBuilder()
                .SetIssuer(issuer.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(localCertificate.GetCertificate(_workContext))
                .Build();

            token.Should().NotBeNullOrEmpty();

            var localCertificate2 = new LocalCertificate(StoreLocation.LocalMachine, StoreName.My, "A4A261513C973CCCC13ABA45B2062484F71CE32F", true);

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(localCertificate2.GetCertificate(_workContext))
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(_workContext, token);

            // Failure
            tokenDetails.Should().BeNull();

            tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(localCertificate.GetCertificate(_workContext))
                .AddCertificate(localCertificate2.GetCertificate(_workContext))
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(_workContext, token);

            tokenDetails.Should().NotBeNull();
            tokenDetails.JwtSecurityToken.Header.Kid.Should().Be(localCertificate.LocalCertificateKey.Thumbprint);
        }

        [Fact]
        public void JwtSecuritySignatureFailureTest()
        {
            var issuer = new Uri("http://localhost/Issuer");
            var localCertificate = new LocalCertificate(StoreLocation.LocalMachine, StoreName.My, "7A270477C5F0B9AAB2AD304B0838E1F8714C5377", true);

            string token = new JwtTokenBuilder()
                .SetIssuer(issuer.ToString())
                .SetExpires(DateTime.Now.AddDays(10))
                .SetIssuedAt(DateTime.Now)
                .SetClaim(new Claim("Email", "testemail@domain.com"))
                .SetCertificate(localCertificate.GetCertificate(_workContext))
                .Build();

            token.Should().NotBeNullOrEmpty();

            token = token.Remove(3, 2);

            JwtTokenDetails tokenDetails = new JwtTokenParserBuilder()
                .AddCertificate(localCertificate.GetCertificate(_workContext))
                .AddValidIssuer(issuer.ToString())
                .Build()
                .Parse(_workContext, token);

            tokenDetails.Should().BeNull();
        }
    }
}
