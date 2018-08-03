// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Security;
using System.Collections.Generic;

namespace Khooversoft.Toolbox.Services
{
    public interface IServerTokenManagerConfiguration
    {
        IEnumerable<string> ValidIssuers { get; }

        IEnumerable<LocalCertificateKey> TokenAuthorizationRequestCertificateKeys { get; }

        TokenAuthorizationConfiguration TokenAuthorization { get; }

        IHmacConfiguration HmacConfiguration { get; }
    }
}
