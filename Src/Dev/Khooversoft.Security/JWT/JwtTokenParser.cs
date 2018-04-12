// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Security
{
    /// <summary>
    /// JWT Token parser - designed to parse and validate many tokens based on the same set of issuers
    /// and same set of audiences
    /// 
    /// Parse a token and does validation based on issuers and audiences.
    /// 
    /// Search a collection of certificates for thumbprint that matches key id (KID) in the JWT token's header
    /// </summary>
    public class JwtTokenParser
    {
        public JwtTokenParser(IEnumerable<KeyValuePair<string, X509Certificate2>> certificates, IEnumerable<string> validIssuers, IEnumerable<string> validAudiences)
        {
            Verify.IsNotNull(nameof(certificates), certificates);
            Verify.IsNotNull(nameof(validIssuers), validIssuers);
            Verify.IsNotNull(nameof(validAudiences), validAudiences);

            Certificates = certificates.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
            ValidIssuers = new List<string>(validIssuers);
            ValidAudiences = new List<string>(validAudiences);
        }

        /// <summary>
        /// List of X509 certificates that can be used to verify signature.  The field "KID" in the header specified
        /// which certificate thumbprint was used to create the signature.
        /// 
        /// If JWT token does not specify a KID field, then the token is parsed and returned.  Not signature validation
        /// is performed.
        /// </summary>
        public IReadOnlyDictionary<string, X509Certificate2> Certificates { get; }

        /// <summary>
        /// List valid JWT issuers (can be empty list).  If specified, will be used to verify JWT.
        /// </summary>
        public IReadOnlyList<string> ValidIssuers { get; }

        /// <summary>
        /// List of valid audiences (can be empty list).  If specified, will be used to verify JWT
        /// </summary>
        public IReadOnlyList<string> ValidAudiences { get; }

        /// <summary>
        /// Parse JWT token to details
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="token">JWT token</param>
        /// <returns>token details or null</returns>
        public JwtTokenDetails Parse(IWorkContext context, string token)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(token), token);

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

                if ((jwtToken?.Header.Kid).IsEmpty())
                {
                    return new JwtTokenDetails(jwtToken);
                }

                if (!Certificates.TryGetValue(jwtToken.Header.Kid, out X509Certificate2 certificate))
                {
                    return null;
                }

                var securityKey = new X509SecurityKey(certificate);

                var validation = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = ValidIssuers.Count > 0,
                    ValidIssuers = ValidIssuers.Count > 0 ? ValidIssuers : null,
                    ValidateAudience = ValidAudiences.Count > 0,
                    ValidAudiences = ValidAudiences.Count > 0 ? ValidAudiences : null,
                    IssuerSigningKey = securityKey,
                };

                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validation, out SecurityToken securityToken);
                return new JwtTokenDetails(jwtToken, securityToken, claimsPrincipal);
            }
            catch (Exception ex)
            {
                SecurityEventSource.Log.Error(context, "Parse JWT token failure", ex);
                return null;
            }
        }
    }
}
