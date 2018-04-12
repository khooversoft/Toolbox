// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Actor;
using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public interface IIdentityActor : IActor
    {
        Task Set(IWorkContext context, IdentityPrincipal identityPrincipal);

        Task<IdentityPrincipal> Get(IWorkContext context);

        Task<IdentityPrincipal> Remove(IWorkContext context);
    }
}
