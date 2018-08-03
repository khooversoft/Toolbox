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

namespace Khooversoft.Toolbox.Security
{
    /// <summary>
    /// Build JWT token (builder pattern)
    /// </summary>
    public class JwtTokenBuilder
    {
        public JwtTokenBuilder()
        {
        }

        public X509Certificate2 Certificate { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public IList<Claim> Claims { get; } = new List<Claim>();

        public DateTime? NotBefore { get; set; }
        
        public DateTime? Expires { get; set; }

        public DateTime? IssuedAt { get; set; }

        public string WebKey { get; set; }

        public JwtTokenBuilder SetCertificate(X509Certificate2 certificate)
        {
            Certificate = certificate;
            return this;
        }

        public JwtTokenBuilder AddSubject(string subject)
        {
            Verify.IsNotEmpty(nameof(subject), subject);

            Claims.Add(new Claim(JwtStandardClaimNames.SubjectName, subject));
            return this;
        }

        public JwtTokenBuilder SetIssuer(string issuer)
        {
            Issuer = issuer;
            return this;
        }

        public JwtTokenBuilder SetAudience(string audience)
        {
            Audience = audience;
            return this;
        }

        public JwtTokenBuilder SetNotBefore(DateTime? notBefore)
        {
            NotBefore = notBefore;
            return this;
        }

        public JwtTokenBuilder SetExpires(DateTime? expires)
        {
            Expires = expires;
            return this;
        }

        public JwtTokenBuilder SetIssuedAt(DateTime? issuedAt)
        {
            IssuedAt = issuedAt;
            return this;
        }

        public JwtTokenBuilder SetClaim(Claim claim)
        {
            Verify.IsNotNull(nameof(claim), claim);

            Claims.Add(claim);
            return this;
        }

        public JwtTokenBuilder SetWebKey(string webKey)
        {
            WebKey = webKey;
            return this;
        }

        public string Build()
        {
            Verify.IsNotNull(nameof(Certificate), Certificate);
            var securityKey = new X509SecurityKey(Certificate);

            var header = new JwtHeader(new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha512));

            var addClaims = new List<Claim>();
            if( WebKey.IsNotEmpty() )
            {
                addClaims.Add(new Claim(JwtStandardClaimNames.WebKeyName, WebKey));
            };

            var payload = new JwtPayload(Issuer, Audience, Claims.Concat(addClaims), NotBefore, Expires, IssuedAt);

            var jwtToken = new JwtSecurityToken(header, payload);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(jwtToken);
        }
    }
}
