// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Threading;

namespace Khooversoft.Actor
{
    /// <summary>
    /// Actor registration information
    /// </summary>
    public class ActorRegistration : IActorRegistration
    {
        private IActorBase _instance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actorType">actor type</param>
        /// <param name="actorKey">actor key</param>
        /// <param name="instance">instance of the actor class</param>
        /// <param name="actorProxy">actor proxy</param>
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

        /// <summary>
        /// Actor key
        /// </summary>
        public ActorKey ActorKey { get; }

        /// <summary>
        /// Actor instance
        /// </summary>
        public IActorBase Instance { get { return _instance; } }

        /// <summary>
        /// Type of actor
        /// </summary>
        public Type ActorType { get; }

        /// <summary>
        /// Proxy to actor
        /// </summary>
        public IActor ActorProxy { get; private set; }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ActorProxy = null;

            IActorBase current = Interlocked.Exchange(ref _instance, null);
            current?.Dispose();
        }

        /// <summary>
        /// Get instance
        /// </summary>
        /// <typeparam name="T">actor type</typeparam>
        /// <returns>instance of actor</returns>
        public T GetInstance<T>() where T : IActor
        {
            return (T)ActorProxy;
        }
    }
}
