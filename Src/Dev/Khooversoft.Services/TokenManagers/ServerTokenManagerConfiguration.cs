// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Security;
using System.Collections.Generic;

namespace Khooversoft.Services
{
    public class ServerTokenManagerConfiguration : IServerTokenManagerConfiguration
    {
        public ServerTokenManagerConfiguration()
        {
        }

        public IEnumerable<string> ValidIssuers { get; set; }

        public IEnumerable<LocalCertificateKey> TokenAuthorizationRequestCertificateKeys { get; set; }

        public TokenAuthorizationConfiguration TokenAuthorization { get; set; }

        public IHmacConfiguration HmacConfiguration { get; set; }
    }
}
