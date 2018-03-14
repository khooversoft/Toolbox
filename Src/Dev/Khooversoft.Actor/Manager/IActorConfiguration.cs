using Autofac;
using System;

namespace Khooversoft.Actor
{
    public interface IActorConfiguration
    {
        int Capacity { get; }

        IActorRepository ActorRepository { get; }

        ILifetimeScope Container { get; }

        TimeSpan ActorCallTimeout { get; }

        TimeSpan ActorRetirementPeriod { get; }

        TimeSpan InactivityScanPeriod { get; }
    }
}
