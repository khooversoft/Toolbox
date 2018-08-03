// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    public interface IActorManager : IDisposable
    {
        IActorManager Register<T>(IWorkContext context, Func<IWorkContext, ActorKey, IActorManager, T> createImplementation) where T : IActor;

        Task<T> CreateProxyAsync<T>(IWorkContext context, ActorKey actorKey) where T : IActor;

        Task<bool> DeactivateAsync<T>(IWorkContext context, ActorKey actorKey);

        Task DeactivateAllAsync(IWorkContext context);

        IActorConfiguration Configuration { get; }
    }
}
