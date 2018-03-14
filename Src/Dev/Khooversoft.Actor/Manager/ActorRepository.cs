using Khooversoft.Toolbox;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Actor
{
    /// <summary>
    /// Stores actor registrations.  When an actor registration is added or removed, the actor's activate and deactivate methods are called
    /// </summary>
    public class ActorRepository : IActorRepository
    {
        private static Guid _cvKey = new Guid("{6E481C78-BB9D-4454-8EDC-0C3868A6A275}");
        private readonly IWorkContext _workContext;
        private readonly Tag _tag = new Tag(nameof(ActorRepository));

        private readonly LruCache<RegistrationKey, IActorRegistration> _actors;
        private readonly ConcurrentQueue<IActorRegistration> _actorRemovedQueue = new ConcurrentQueue<IActorRegistration>();
        private readonly object _lock = new object();
        private int _timerLockValue;
        private readonly Timer _timer;
        private readonly IActorConfiguration _configuration;

        public ActorRepository(IActorConfiguration configuration)
        {
            Verify.IsNotNull(nameof(configuration), configuration);
            Verify.Assert(configuration.Capacity > 0, $"Capacity {configuration.Capacity} must be greater than 0");

            _workContext = new WorkContextBuilder()
                .Set(_tag)
                .Set(new CorrelationVector(_cvKey))
                .Build();

            _configuration = configuration;

            _actors = new LruCache<RegistrationKey, IActorRegistration>(_configuration.Capacity, new RegistrationKeyComparer());
            _actors.CacheItemRemoved += x => _actorRemovedQueue.Enqueue(x.Value);

            _timer = new Timer(GarbageCollection, null, _configuration.InactivityScanPeriod, _configuration.InactivityScanPeriod);
        }

        /// <summary>
        /// Clear all actors from the system.  Each active actor will be deactivated
        /// </summary>
        /// <param name="context">context</param>
        /// <returns></returns>
        public async Task ClearAsync(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            ActorEventSource.Log.Info(context, "Clearing actor container");
            List<IActorRegistration> list;

            lock (_lock)
            {
                list = new List<IActorRegistration>(_actors.Values);
                _actors.Clear();
            }

            foreach (var item in list)
            {
                await item.Instance.DeactivateAsync(context);
                item.Instance.Dispose();
            }
        }

        /// <summary>
        /// Set actor (add or replace)
        /// </summary>
        /// <param name="registration">actor registration</param>
        /// <returns>task</returns>
        public async Task SetAsync(IWorkContext context, IActorRegistration registration)
        {
            Verify.IsNotNull(nameof(registration), registration);
            context = context.WithTag(_tag);

            ActorEventSource.Log.Verbose(context, $"Setting actor {registration.ActorKey}");
            IActorRegistration currentActorRegistration = null;

            var key = new RegistrationKey(registration.ActorType, registration.ActorKey.Key);

            lock (_lock)
            {
                if (!_actors.TryRemove(key, out currentActorRegistration))
                {
                    currentActorRegistration = null;
                }

                _actors.Set(key, registration);
            }

            if (currentActorRegistration != null)
            {
                await currentActorRegistration.Instance.DeactivateAsync(context);
                currentActorRegistration.Instance.Dispose();
            }

            await registration.Instance.ActivateAsync(context);
        }

        /// <summary>
        /// Lookup actor
        /// </summary>
        /// <param name="actorType">actor type</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>actor registration or null if not exist</returns>
        public IActorRegistration Lookup(Type actorType, ActorKey actorKey)
        {
            Verify.IsNotNull(nameof(actorType), actorType);
            Verify.IsNotNull(nameof(actorKey), actorKey);

            lock (_lock)
            {
                var key = new RegistrationKey(actorType, actorKey.Key);
                if (_actors.TryGetValue(key, out IActorRegistration registration))
                {
                    return registration;
                }

                return null;
            }
        }

        /// <summary>
        /// Remove actor from container
        /// </summary>
        /// <param name="actorType">actor type</param>
        /// <param name="actorKey">actor key</param>
        /// <returns>actor registration or null if not exist</returns>
        public async Task<IActorRegistration> RemoveAsync(IWorkContext context, Type actorType, ActorKey actorKey)
        {
            Verify.IsNotNull(nameof(actorType), actorType);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            context = context.WithTag(_tag);

            ActorEventSource.Log.Verbose(context, $"Removing actor {actorKey}");

            IActorRegistration registration;

            lock (_lock)
            {
                var key = new RegistrationKey(actorType, actorKey.Key);
                if (!_actors.TryRemove(key, out registration))
                {
                    return null;
                }
            }

            await registration.Instance.DeactivateAsync(context);
            registration.Instance.Dispose();
            return registration;
        }

        /// <summary>
        /// Scan current actors for inactivity equal or greater than retirement period
        /// </summary>
        /// <param name="obj">not used</param>
        private void GarbageCollection(object obj)
        {
            int currentValue = Interlocked.CompareExchange(ref _timerLockValue, 1, 0);
            if (currentValue != 0)
            {
                return;
            }

            try
            {
                ActorEventSource.Log.Verbose(_workContext.WithMethodName(), "GarbageCollection");
                DateTimeOffset retireDate = DateTimeOffset.UtcNow.AddSeconds(-_configuration.ActorRetirementPeriod.TotalSeconds);

                foreach (var item in _actors)
                {
                    if (item.LastAccessed < retireDate)
                    {
                        _actorRemovedQueue.Enqueue(item.Value);
                    }
                }

                RetireActorAsync().GetAwaiter().GetResult();
            }
            finally
            {
                Interlocked.Exchange(ref _timerLockValue, 0);
            }
        }

        /// <summary>
        /// Loop through remove queue and remove actors
        /// </summary>
        /// <returns>task</returns>
        private async Task RetireActorAsync()
        {
            while (_actorRemovedQueue.Count > 0)
            {
                if (!_actorRemovedQueue.TryDequeue(out IActorRegistration registration))
                {
                    continue;
                }

                await RemoveAsync(_workContext, registration.ActorType, registration.ActorKey);
            }
        }

        /// <summary>
        /// Registration key so we can use a single dictionary
        /// </summary>
        private class RegistrationKey
        {
            public RegistrationKey(Type type, Guid key)
            {
                Verify.IsNotNull(nameof(type), type);

                Type = type;
                Key = key;
            }

            public Type Type { get; }

            public Guid Key { get; }
        }

        /// <summary>
        /// Key compare
        /// </summary>
        private class RegistrationKeyComparer : IEqualityComparer<RegistrationKey>
        {
            public bool Equals(RegistrationKey x, RegistrationKey y)
            {
                return x.Type == y.Type &&
                    x.Key == y.Key;
            }

            public int GetHashCode(RegistrationKey obj)
            {
                return obj.Type.GetHashCode() ^ obj.Key.GetHashCode();
            }
        }
    }
}
