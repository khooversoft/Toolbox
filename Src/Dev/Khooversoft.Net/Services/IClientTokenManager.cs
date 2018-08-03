// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Security;
using System.Threading.Tasks;

namespace Khooversoft.Net
{
    public interface IClientTokenManager
    {
        Task<string> CreateRequestToken(IWorkContext context, string subject = null);

        Task<RestResponse<string>> RequestServerAuthorizationToken(IWorkContext context, string requestToken);

        Task<JwtTokenDetails> ParseAuthorizationToken(IWorkContext context, string authorizationToken);
    }
}
