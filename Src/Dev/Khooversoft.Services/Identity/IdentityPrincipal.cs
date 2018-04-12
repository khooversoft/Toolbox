// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Khooversoft.Services
{
    /// <summary>
    /// Identity principal details
    /// </summary>
    public class IdentityPrincipal : IIdentityPrincipal
    {
        private readonly Dictionary<string, string> _properties;

        public IdentityPrincipal(PrincipalId principalId, IdentityPrincipalType principalType)
        {
            Verify.IsNotEmpty(nameof(principalId), principalId);

            PrincipalId = principalId;
            PrincipalType = principalType;
            _properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public IdentityPrincipal(PrincipalId clientId, IdentityPrincipalType principalType, ApiKey apiKey, IEnumerable<KeyValuePair<string, string>> properties)
        {
            Verify.IsNotEmpty(nameof(clientId), clientId);

            PrincipalId = clientId;
            PrincipalType = principalType;
            ApiKey = apiKey;

            properties = properties ?? Enumerable.Empty<KeyValuePair<string, string>>();
            _properties = properties.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        public PrincipalId PrincipalId { get; }

        public ApiKey ApiKey { get; private set; }

        public IdentityPrincipalType PrincipalType { get; private set; }

        public IdentityPrincipal Clone()
        {
            return new IdentityPrincipal(PrincipalId, PrincipalType, ApiKey, _properties);
        }

        public IdentityPrincipal With(ApiKey apiKey)
        {
            Verify.IsNotEmpty(nameof(apiKey), apiKey);

            return new IdentityPrincipal(PrincipalId, PrincipalType, apiKey, _properties);
        }
    }
}
