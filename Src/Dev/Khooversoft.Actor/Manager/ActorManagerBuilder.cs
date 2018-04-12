// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using System;

namespace Khooversoft.Actor
{
    public class ActorManagerBuilder : IActorConfiguration
    {
        public ActorManagerBuilder()
        {
        }

        public int Capacity { get; private set; } = 10000;

        public IActorRepository ActorRepository { get; private set; }

        public ILifetimeScope Container { get; private set; }

        public TimeSpan ActorCallTimeout { get; private set; } = TimeSpan.FromSeconds(120);

        public TimeSpan ActorRetirementPeriod { get; private set; } = TimeSpan.FromMinutes(60);

        public TimeSpan InactivityScanPeriod { get; private set; } = TimeSpan.FromMinutes(5);

        public ActorManagerBuilder Set(IActorRepository repository)
        {
            ActorRepository = repository;
            return this;
        }

        public ActorManagerBuilder Set(ILifetimeScope container)
        {
            Container = container;
            return this;
        }

        public ActorManagerBuilder SetActorCallTimeout(TimeSpan span)
        {
            ActorCallTimeout = span;
            return this;
        }

        public ActorManagerBuilder SetActorRetirementPeriod(TimeSpan span)
        {
            ActorRetirementPeriod = span;
            return this;
        }

        public ActorManagerBuilder SetInactivityScanPeriod(TimeSpan span)
        {
            InactivityScanPeriod = span;
            return this;
        }

        public IActorManager Build()
        {
            var configuration = new ActorManagerBuilder
            {
                Capacity = this.Capacity,
                ActorRepository = this.ActorRepository,
                Container = this.Container,
                ActorCallTimeout = this.ActorCallTimeout,
                ActorRetirementPeriod = this.ActorRetirementPeriod,
                InactivityScanPeriod = this.InactivityScanPeriod,
            };

            return new ActorManager(configuration);
        }
    }
}
