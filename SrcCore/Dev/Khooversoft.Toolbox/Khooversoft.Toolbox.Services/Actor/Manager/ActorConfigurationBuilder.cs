using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Actor
{
    public class ActorConfigurationBuilder
    {
        public ActorConfigurationBuilder()
        {
        }

        public ActorConfigurationBuilder(IActorConfiguration configuration)
        {
            Verify.IsNotNull(nameof(configuration), configuration);

            Capacity = configuration.Capacity;
            ActorRepository = configuration.ActorRepository;
            ActorCallTimeout = configuration.ActorCallTimeout;
            ActorRetirementPeriod = configuration.ActorRetirementPeriod;
            InactivityScanPeriod = configuration.InactivityScanPeriod;
            Registration = configuration.Registration;
            WorkContext = configuration.WorkContext;
        }

        public int Capacity { get; private set; } = 10000;

        public IActorRepository ActorRepository { get; private set; }

        public TimeSpan ActorCallTimeout { get; private set; } = TimeSpan.FromSeconds(120);

        public TimeSpan ActorRetirementPeriod { get; private set; } = TimeSpan.FromMinutes(60);

        public TimeSpan InactivityScanPeriod { get; private set; } = TimeSpan.FromMinutes(5);

        public IList<ActorTypeRegistration> Registration { get; } = new List<ActorTypeRegistration>();

        public IWorkContext WorkContext { get; private set; }

        public ActorConfigurationBuilder Set(int capacity)
        {
            Capacity = capacity;
            return this;
        }

        public ActorConfigurationBuilder Set(IActorRepository repository)
        {
            ActorRepository = repository;
            return this;
        }

        public ActorConfigurationBuilder Set(IWorkContext context)
        {
            WorkContext = context;
            return this;
        }

        public ActorConfigurationBuilder Register<T>(Func<IWorkContext, ActorKey, IActorManager, T> createImplementation) where T : IActor
        {
            Verify.IsNotNull(nameof(createImplementation), createImplementation);

            Registration.Add(new ActorTypeRegistration(typeof(T), (c, x, m) => createImplementation(c, x, m)));
            return this;
        }

        public ActorConfigurationBuilder SetActorCallTimeout(TimeSpan span)
        {
            ActorCallTimeout = span;
            return this;
        }

        public ActorConfigurationBuilder SetActorRetirementPeriod(TimeSpan span)
        {
            ActorRetirementPeriod = span;
            return this;
        }

        public ActorConfigurationBuilder SetInactivityScanPeriod(TimeSpan span)
        {
            InactivityScanPeriod = span;
            return this;
        }

        public IActorConfiguration Build()
        {
            return new ActorConfiguration(
                capacity: Capacity,
                actorRepository: ActorRepository,
                actorCallTimeout: ActorCallTimeout,
                actorRetirementPeriod: ActorRetirementPeriod,
                inactivityScanPeriod: InactivityScanPeriod,
                registrations: Registration,
                workContext: WorkContext
            );
        }
    }

    public static class ActorConfigurationBuilderExtensions
    {
        public static IActorManager ToActorManager(this IActorConfiguration self)
        {
            if( self == null )
            {
                return null;
            }

            return new ActorManager(self);
        }
    }
}
