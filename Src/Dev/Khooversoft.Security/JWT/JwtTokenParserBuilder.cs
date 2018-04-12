// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Security
{
    /// <summary>
    /// Build a JWT token parser, specify certificates, audiences, and issuers.  This is just a helper builder pattern class
    /// </summary>
    public class JwtTokenParserBuilder
    {
        public JwtTokenParserBuilder()
        {
        }

        public IDictionary<string, X509Certificate2> Certificates { get; } = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

        public IList<string> ValidIssuers { get; set; } = new List<string>();

        public IList<string> ValidAudiences { get; } = new List<string>();

        public JwtTokenParserBuilder Clear()
        {
            ValidIssuers.Clear();
            Certificates.Clear();
            ValidAudiences.Clear();

            return this;
        }

        public JwtTokenParserBuilder AddValidIssuer(string validIssuer)
        {
            ValidIssuers.Add(validIssuer);
            return this;
        }

        public JwtTokenParserBuilder AddValidIssuers(IEnumerable<string> validIssuers)
        {
            Verify.IsNotNull(nameof(validIssuers), validIssuers);

            validIssuers.Run(x => AddValidIssuer(x));
            return this;
        }

        public JwtTokenParserBuilder AddCertificate(X509Certificate2 certificate)
        {
            Verify.IsNotNull(nameof(certificate), certificate);

            Certificates.Add(certificate.Thumbprint, certificate);
            return this;
        }

        public JwtTokenParserBuilder AddCertificates(IEnumerable<X509Certificate2> certificates)
        {
            Verify.IsNotNull(nameof(certificates), certificates);

            certificates.Run(x => AddCertificate(x));
            return this;
        }

        public JwtTokenParserBuilder AddValidAudience(string validAudience)
        {
            Verify.IsNotEmpty(nameof(validAudience), validAudience);

            ValidAudiences.Add(validAudience);
            return this;
        }

        public JwtTokenParser Build()
        {
            return new JwtTokenParser(Certificates, ValidIssuers, ValidAudiences);
        }
    }
}
