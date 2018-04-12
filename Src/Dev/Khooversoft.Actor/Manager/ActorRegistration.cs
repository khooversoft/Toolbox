// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Threading;

namespace Khooversoft.Actor
{
    public class ActorRegistration : IActorRegistration
    {
        private IActorBase _instance;

        public ActorRegistration(Type actorType, ActorKey actorKey, IActorBase instance, IActor actorProxy)
        {
            Verify.IsNotNull(nameof(actorType), actorType);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(instance), instance);
            Verify.IsNotNull(nameof(actorProxy), actorProxy);

            ActorType = actorType;
            ActorKey = actorKey;
            _instance = instance;
            ActorProxy = actorProxy;
        }

        public ActorKey ActorKey { get; }

        public IActorBase Instance { get { return _instance; } }

        public Type ActorType { get; }

        public IActor ActorProxy { get; private set; }

        public void Dispose()
        {
            ActorProxy = null;

            IActorBase current = Interlocked.Exchange(ref _instance, null);
            current?.Dispose();
        }

        public T GetInstance<T>() where T : IActor
        {
            return (T)ActorProxy;
        }
    }
}
