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
        /// Register actor for activator creation
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <typeparam name="TBase">actor implementation</typeparam>
        /// <param name="context">context</param>
        /// <returns>this</returns>
        public ActorTypeManager Register<T, TBase>(IWorkContext context)
            where TBase : IActorBase
            where T : IActor
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.Assert(typeof(T).IsInterface, $"{typeof(T).FullName} must be an interface");
            context = context.WithTag(_tag);

            _actorRegistration.AddOrUpdate(typeof(T),
                x => new ActorTypeRegistration(x, typeof(TBase)),
                (x, old) => new ActorTypeRegistration(x, typeof(TBase)));

            ActorEventSource.Log.ActorRegistered(context, typeof(T), "interface registered");
            return this;
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

            IActor actorObject = typeRegistration.CreateImplementation != null ?
                typeRegistration.CreateImplementation(context, actorKey, manager) :
                (IActor)Activator.CreateInstance(typeRegistration.ActorType, actorKey, manager);

            return (T)actorObject;
        }
    }
}
