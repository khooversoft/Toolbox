// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using Khooversoft.Toolbox;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    public class ActorManager : IActorManager, IDisposable
    {
        private static Tag _tag = new Tag(nameof(ActorManager));
        private static IWorkContext _actorManagerWorkContext = new WorkContextBuilder().Set(_tag).Build();

        private readonly IActorRepository _actorRepository;
        private readonly ActorTypeManager _typeManager = new ActorTypeManager();
        private int _disposed;
        private bool _disposing = false;
        private const string _disposedTestText = "Actor Manager has been disposed";

        public ActorManager()
            : this(ActorConfiguration.Default)
        {
        }

        public ActorManager(IActorConfiguration configuration)
        {
            Verify.IsNotNull(nameof(configuration), configuration);

            Configuration = configuration;
            _actorRepository = Configuration.ActorRepository ?? new ActorRepository(Configuration);

            foreach (ActorTypeRegistration registration in configuration?.Registration ?? Enumerable.Empty<ActorTypeRegistration>())
            {
                _typeManager.Register(_actorManagerWorkContext, registration);
            }
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IActorConfiguration Configuration { get; private set; }

        /// <summary>
        /// Container setup for Actors
        /// </summary>
        //public ILifetimeScope Container => Configuration.Container;

        /// <summary>
        /// Is actor manager running (not disposed)
        /// </summary>
        public bool IsRunning { get { return _disposed == 0 || _disposing; } }

        /// <summary>
        /// Register actor and lambda creator
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="createImplementation">creator</param>
        /// <returns>this</returns>
        public IActorManager Register<T>(IWorkContext context, Func<IWorkContext, ActorKey, IActorManager, T> createImplementation) where T : IActor
        {
            Verify.Assert(IsRunning, _disposedTestText);
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(createImplementation), createImplementation);

            _typeManager.Register<T>(context.WithTag(_tag), createImplementation);
            return this;
        }

        /// <summary>
        /// Create virtual actor, return current instance or create one
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>actor proxy interface</returns>
        public async Task<T> CreateProxyAsync<T>(IWorkContext context, ActorKey actorKey) where T : IActor
        {
            Verify.Assert(IsRunning, _disposedTestText);

            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            context = context.WithTag(_tag);

            Type actorType = typeof(T);

            // Lookup instance of actor (type + actorKey)
            IActorRegistration actorRegistration = _actorRepository.Lookup(actorType, actorKey);
            if (actorRegistration != null)
            {
                return actorRegistration.GetInstance<T>();
            }

            // Create actor
            IActor actorObject = _typeManager.Create<T>(context, actorKey, this);

            IActorBase actorBase = actorObject as IActorBase;
            if (actorBase == null)
            {
                var ex = new ArgumentException($"Actor {actorObject.GetType().FullName} does not implement IActorBase");
                Configuration.WorkContext.EventLog.Error(context, "Cannot create", ex);
                throw ex;
            }

            T actorInterface = ActorProxy<T>.Create(context, actorBase, this);
            actorRegistration = new ActorRegistration(typeof(T), actorKey, actorBase, actorInterface);

            await _actorRepository.SetAsync(context, actorRegistration);

            // Create proxy for interface
            return actorRegistration.GetInstance<T>();
        }

        /// <summary>
        /// Deactivate actor
        /// </summary>
        /// <typeparam name="T">actor interface</typeparam>
        /// <param name="context">context</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>true if deactivated, false if not found</returns>
        public async Task<bool> DeactivateAsync<T>(IWorkContext context, ActorKey actorKey)
        {
            Verify.Assert(IsRunning, _disposedTestText);
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorKey), actorKey);

            IActorRegistration actorRegistration = await _actorRepository.RemoveAsync(context, typeof(T), actorKey);
            if (actorRegistration == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deactivate all actors
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        public async Task DeactivateAllAsync(IWorkContext context)
        {
            Verify.Assert(IsRunning, _disposedTestText);
            Verify.IsNotNull(nameof(context), context);

            await _actorRepository.ClearAsync(context);
        }

        /// <summary>
        /// Dispose of all resources, actors are deactivated, DI container is disposed
        /// </summary>
        public void Dispose()
        {
            try
            {
                _disposing = true;
                int testDisposed = Interlocked.CompareExchange(ref _disposed, 1, 0);
                if (testDisposed == 0)
                {
                    Task.Run(() => _actorRepository.ClearAsync(_actorManagerWorkContext))
                        .ConfigureAwait(false)
                        .GetAwaiter();
                }
            }
            finally
            {
                _disposing = false;
            }
        }
    }
}
