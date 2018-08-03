// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Security;
using Khooversoft.Toolbox.Services;
using System.Threading.Tasks;

namespace Khooversoft.Net
{
    /// <summary>
    /// Token client actor, cache and retrieve updated token from authorization server
    /// </summary>
    public class TokenClientActor : ActorBase, ITokenClientActor
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IRestClientConfiguration _restClientConfiguration;
        private IClientTokenManagerConfiguration _clientTokenManagerConfiguration;
        private IClientTokenManager _clientTokenManager;
        private JwtTokenDetails _jwtTokenDetails;

        public TokenClientActor(ActorKey actorKey, IActorManager actorManager, ICertificateRepository certificateRepository, IRestClientConfiguration restClientConfiguration)
            : base(actorKey, actorManager)
        {
            Verify.IsNotNull(nameof(certificateRepository), certificateRepository);
            Verify.IsNotNull(nameof(restClientConfiguration), restClientConfiguration);

            _certificateRepository = certificateRepository;
            _restClientConfiguration = restClientConfiguration;
        }

        public TokenClientActor(
            ActorKey actorKey,
            IActorManager actorManager,
            ICertificateRepository certificateRepository,
            IRestClientConfiguration restClientConfiguration,
            IClientTokenManagerConfiguration clientTokenManagerConfiguration)
            : this(actorKey, actorManager, certificateRepository, restClientConfiguration)
        {
            Verify.IsNotNull(nameof(clientTokenManagerConfiguration), clientTokenManagerConfiguration);

            _clientTokenManagerConfiguration = clientTokenManagerConfiguration;
            _clientTokenManager = new ClientTokenManager(_certificateRepository, _clientTokenManagerConfiguration, _restClientConfiguration);
        }

        public Task SetConfiguration(IWorkContext context, IClientTokenManagerConfiguration clientTokenManagerConfiguration)
        {
            Verify.IsNotNull(nameof(clientTokenManagerConfiguration), clientTokenManagerConfiguration);
            Verify.Assert(clientTokenManagerConfiguration.TokenKey.Value == ActorKey.VectorKey, "Configuration does not match actor key");

            _clientTokenManagerConfiguration = clientTokenManagerConfiguration;

            _clientTokenManager = new ClientTokenManager(_certificateRepository, _clientTokenManagerConfiguration, _restClientConfiguration);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Get token, return cache or get new token
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>Api key</returns>
        public async Task<string> GetApiKey(IWorkContext context)
        {
            Verify.Assert(_clientTokenManager != null, "Configuration has not been set");

            if (!(await Validate(context)))
            {
                return null;
            }

            return _jwtTokenDetails.ApiKey;
        }

        /// <summary>
        /// Validate current JWT server authorization token and renew if expired
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>true if authorization server's authorization token is valid, false if not</returns>
        private async Task<bool> Validate(IWorkContext context, bool renewIfRequired = true)
        { 
            Verify.IsNotNull(nameof(context), context);

            if (_jwtTokenDetails != null && !_jwtTokenDetails.IsExpired)
            {
                return true;
            }

            string requestToken = await _clientTokenManager.CreateRequestToken(context);
            if (requestToken == null || !renewIfRequired)
            {
                return false;
            }

            RestResponse<string> response = await _clientTokenManager.RequestServerAuthorizationToken(context, requestToken);
            response.AssertSuccessStatusCode(context);
            Verify.IsNotNull(nameof(response.Value), response.Value);

            JwtTokenDetails jwtToken = await _clientTokenManager.ParseAuthorizationToken(context, response.Value);

            _jwtTokenDetails = jwtToken;
            return true;
        }
    }
}
