// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Net;
using Khooversoft.Security;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    /// <summary>
    /// Token client manager provides support for creating and requesting authorization JWT tokens.
    /// 
    /// To support multiple certificates (rolling certificates), 
    /// </summary>
    public class ClientTokenManager : IClientTokenManager
    {
        private readonly Tag _tag = new Tag(nameof(ClientTokenManager));
        private readonly ICertificateRepository _certificateRepository;
        private readonly IClientTokenManagerConfiguration _clientTokenManagerConfiguration;
        private readonly IRestClientConfiguration _restClientConfiguration;

        /// <summary>
        /// Constructor to use default HTTP message handler for REST calls
        /// </summary>
        /// <param name="certificateRepository">certificate repository</param>
        public ClientTokenManager(ICertificateRepository certificateRepository, IClientTokenManagerConfiguration clientTokenManagerConfiguration)
        {
            Verify.IsNotNull(nameof(certificateRepository), certificateRepository);
            Verify.IsNotNull(nameof(clientTokenManagerConfiguration), clientTokenManagerConfiguration);

            _certificateRepository = certificateRepository;
            _clientTokenManagerConfiguration = clientTokenManagerConfiguration;
        }

        /// <summary>
        /// Constructor for using a required HTTP message handler for REST calls
        /// </summary>
        /// <param name="certificateRepository">certificate repository</param>
        /// <param name="restClientConfiguration">REST client configuration</param>
        public ClientTokenManager(ICertificateRepository certificateRepository, IClientTokenManagerConfiguration clientTokenManagerConfiguration, IRestClientConfiguration restClientConfiguration)
            : this(certificateRepository, clientTokenManagerConfiguration)
        {
            Verify.IsNotNull(nameof(restClientConfiguration), restClientConfiguration);

            _restClientConfiguration = restClientConfiguration;
        }

        /// <summary>
        /// Client side request for authorization token
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="subject">subject of the request (null to default use client configuration)</param>
        /// <returns>JWT token</returns>
        public async Task<string> CreateRequestToken(IWorkContext context, string subject = null)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            X509Certificate2 certificate = await _certificateRepository.GetCertificate(context, _clientTokenManagerConfiguration.RequestSigningCertificateKey, true);

            subject = subject ?? _clientTokenManagerConfiguration.TokenKey.RequestingSubject;
            Verify.IsNotEmpty(nameof(subject), subject);

            string token = new JwtTokenBuilder()
                .AddSubject(subject)
                .SetIssuer(_clientTokenManagerConfiguration.RequestingIssuer)
                .SetExpires(DateTime.Now.AddMinutes(5))
                .SetIssuedAt(DateTime.Now)
                .SetCertificate(certificate)
                .Build();

            return token;
        }

        /// <summary>
        /// Request server authorization token, which is used in HAMC support for REST calls to the server or servers.
        /// 
        /// Don't raise exceptions when request fails, return null status.
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="authorizationUri">authorization URI that will return a JWT HMAC token</param>
        /// <param name="requestToken"></param>
        /// <returns>response</returns>
        public async Task<RestResponse<string>> RequestServerAuthorizationToken(IWorkContext context, string requestToken)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(requestToken), requestToken);
            context = context.WithTag(_tag);

            return await new RestClient(_restClientConfiguration)
                .SetAbsoluteUri(_clientTokenManagerConfiguration.AuthorizationUri)
                .SetContent(new AuthorizationTokenRequestContractV1 { RequestToken = requestToken })
                .PostAsync(context)
                .ToRestResponseAsync(context)
                .GetContentAsync<string>(context);
        }

        /// <summary>
        /// Parse authorization token received from the server and return details.
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="authorizationToken">authorization token to parse</param>
        /// <param name="issuer">validate against issuer</param>
        /// <returns>JWT details and client identity (client identity can be null if not found in repository)</returns>
        public async Task<JwtTokenDetails> ParseAuthorizationToken(IWorkContext context, string authorizationToken)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(authorizationToken), authorizationToken);
            context = context.WithTag(_tag);

            List<X509Certificate2> certificates = new List<X509Certificate2>();
            foreach (var item in _clientTokenManagerConfiguration.ServerSigningCertificateKeys)
            {
                certificates.Add(await _certificateRepository.GetCertificate(context, item, throwOnNotFound: true));
            }

            try
            {
                JwtTokenDetails details = new JwtTokenParserBuilder()
                    .AddCertificates(certificates)
                    .AddValidIssuer(_clientTokenManagerConfiguration.TokenKey.AuthorizationIssuer)
                    .Build()
                    .Parse(context, authorizationToken);

                return details;
            }
            catch (Exception ex)
            {
                ServicesEventSource.Log.Warning(context, "Exception processing JWT token", ex);
                throw;
            }
        }
    }
}
