// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace Khooversoft.Actor
{
    /// <summary>
    /// Actor type manager for lambda or activator creation
    /// </summary>
    public class ActorTypeManager
    {
        private readonly ConcurrentDictionary<Type, ActorTypeRegistration> _actorRegistration = new ConcurrentDictionary<Type, ActorTypeRegistration>();
        private readonly Tag _tag = new Tag(nameof(ActorManager));

        public ActorTypeManager()
        {
        }

        /// <summary>
        /// Register actor for lambda creation
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="createImplementation">creation lambda</param>
        /// <returns>this</returns>
        public ActorTypeManager Register<T>(IWorkContext context, Func<IWorkContext, ActorKey, IActorManager, T> createImplementation) where T : IActor
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.Assert(typeof(T).IsInterface, $"{typeof(T).FullName} must be an interface");
            context = context.WithTag(_tag);

            Func<IWorkContext, ActorKey, IActorManager, IActor> create = (c, x, m) => createImplementation(c, x, m);

            _actorRegistration.AddOrUpdate(
                typeof(T),
                x => new ActorTypeRegistration(x, create),
                (x, old) => new ActorTypeRegistration(x, create));

            ActorEventSource.Log.ActorRegistered(context, typeof(T), "lambda registered");
            return this;
        }

        /// <summary>
        /// Register type based
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="actorTypeRegistration">actor type registration</param>
        /// <returns></returns>
        public ActorTypeManager Register(IWorkContext context, ActorTypeRegistration actorTypeRegistration)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorTypeRegistration), actorTypeRegistration);
            Verify.Assert(actorTypeRegistration.InterfaceType.IsInterface, $"{actorTypeRegistration.InterfaceType.FullName} must be an interface");
            context = context.WithTag(_tag);

            _actorRegistration.AddOrUpdate(actorTypeRegistration.InterfaceType, x => actorTypeRegistration, (x, old) => actorTypeRegistration);

            ActorEventSource.Log.ActorRegistered(context, actorTypeRegistration.InterfaceType, "lambda registered");
            return this;
        }

        /// <summary>
        /// Create actor from either lambda or activator
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <param name="manager">actor manager</param>
        /// <returns>instance of actor implementation</returns>
        public T Create<T>(IWorkContext context, ActorKey actorKey, IActorManager manager) where T : IActor
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(manager), manager);
            Verify.Assert(typeof(T).IsInterface, $"{typeof(T)} must be an interface");
            context = context.WithTag(_tag);

            Type actorType = typeof(T);

            if (!_actorRegistration.TryGetValue(actorType, out ActorTypeRegistration typeRegistration))
            {
                var ex = new KeyNotFoundException($"Registration for {actorType.FullName} was not found");
                ActorEventSource.Log.Error(context, "create failure", ex);
                throw ex;
            }

            IActor actorObject = typeRegistration.CreateImplementation(context, actorKey, manager);
            return (T)actorObject;
        }
    }
}
