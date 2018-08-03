// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    public interface IActorRepository
    {
        Task ClearAsync(IWorkContext context);

        Task SetAsync(IWorkContext context, IActorRegistration registration);

        IActorRegistration Lookup(Type actorType, ActorKey actorKey);

        Task<IActorRegistration> RemoveAsync(IWorkContext context, Type actorType, ActorKey actorKey);
    }
}
