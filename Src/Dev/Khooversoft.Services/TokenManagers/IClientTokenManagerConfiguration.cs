// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Security;
using System;
using System.Collections.Generic;

namespace Khooversoft.Services
{
    public interface IClientTokenManagerConfiguration
    {
        LocalCertificateKey RequestSigningCertificateKey { get; }

        string RequestingIssuer { get; }

        Uri AuthorizationUri { get; }

        IEnumerable<LocalCertificateKey> ServerSigningCertificateKeys { get; }

        TokenKey TokenKey { get; }

        IHmacConfiguration HmacConfiguration { get; }
    }
}
