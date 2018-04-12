// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;

namespace Khooversoft.Services
{
    public interface IIdentityStore : IIdentityRepository
    {
        IdentityPrincipal Get(IWorkContext context, PrincipalId principalId);

        IdentityInMemoryStore Set(IWorkContext context, IdentityPrincipal identityPrincipal);

        IdentityPrincipal Remove(IWorkContext context, PrincipalId principalId);
    }
}
