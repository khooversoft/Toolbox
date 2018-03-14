using Khooversoft.Security;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    /// <summary>
    /// Server token manager, creates JWT authorization tokens based on identities in the identity repository
    /// </summary>
    public class ServerTokenManager : IServerTokenManager
    {
        private readonly Tag _tag = new Tag(nameof(ServerTokenManager));
        private readonly IIdentityRepository _identityRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IServerTokenManagerConfiguration _configuration;

        public ServerTokenManager(
            IIdentityRepository identityRepository,
            ICertificateRepository certificateRepository,
            IServerTokenManagerConfiguration configuration)
        {
            Verify.IsNotNull(nameof(identityRepository), identityRepository);
            Verify.IsNotNull(nameof(certificateRepository), certificateRepository);
            Verify.IsNotNull(nameof(configuration), configuration);

            _identityRepository = identityRepository;
            _certificateRepository = certificateRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Create JWT token for access to server
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="requestToken"></param>
        /// <returns></returns>
        public async Task<string> CreateAutorizationToken(IWorkContext context, string requestToken)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(requestToken), requestToken);
            context = context.WithTag(_tag);

            var certificateList = new List<X509Certificate2>();
            foreach (var item in _configuration.TokenAuthorizationRequestCertificateKeys)
            {
                certificateList.Add(await _certificateRepository.GetCertificate(context, item, true));
            }

            JwtTokenParser requestTokenParser = new JwtTokenParserBuilder()
                .AddCertificates(certificateList)
                .AddValidIssuers(_configuration.ValidIssuers)
                .Build();

            JwtTokenDetails details = requestTokenParser.Parse(context, requestToken);
            if (details == null || details.JwtSecurityToken.Payload.Sub.IsEmpty())
            {
                ServicesEventSource.Log.Verbose(context, "Payload.Sub is empty");
                return null;
            }

            IdentityPrincipal identity = await _identityRepository.GetAsync(context, new PrincipalId(details.JwtSecurityToken.Payload.Sub));

            // If JWT subject is a valid issuer, then this should be a service principal
            if (_configuration.ValidIssuers.Any(x => x == details.JwtSecurityToken.Payload.Sub))
            {
                if (identity == null)
                {
                    identity = new IdentityPrincipal(new PrincipalId(details.JwtSecurityToken.Payload.Sub), IdentityPrincipalType.Service);
                }
                else
                {
                    if (identity.PrincipalType != IdentityPrincipalType.Service)
                    {
                        ServicesEventSource.Log.Verbose(context, $"Identity {details.JwtSecurityToken.Payload.Sub} is not a service principal");
                        return null;
                    }
                }
            }
            else
            {
                // Identity principal does not exist
                if (identity == null)
                {
                    ServicesEventSource.Log.Verbose(context, $"Identity {details.JwtSecurityToken.Payload.Sub} does not exist");
                    return null;
                }
            }

            DateTime expires = DateTime.UtcNow + _configuration.TokenAuthorization.GoodFor;

            identity = identity.With(ApiKey.CreateApiKey(expires));
            await _identityRepository.SetAsync(context, identity);

            X509Certificate2 certificate = await _certificateRepository.GetCertificate(
                context,
                _configuration.TokenAuthorization.AuthorizationSigningCertificateKey,
                throwOnNotFound: true);

            string token = new JwtTokenBuilder()
                .AddSubject(identity.PrincipalId)
                .SetAudience(_configuration.TokenAuthorization.Audience)
                .SetIssuer(_configuration.TokenAuthorization.Issuer)
                .SetExpires(expires)
                .SetIssuedAt(DateTime.Now)
                .SetWebKey(identity.ApiKey.Value)
                .SetCertificate(certificate)
                .Build();

            return token;
        }
    }
}
