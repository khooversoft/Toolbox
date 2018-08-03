// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;

namespace Khooversoft.Toolbox.Actor
{
    public class ActorManagerBuilder : IActorConfiguration
    {
        public ActorManagerBuilder()
        {
        }

        public int Capacity { get; private set; } = 10000;

        public IActorRepository ActorRepository { get; private set; }

        public TimeSpan ActorCallTimeout { get; private set; } = TimeSpan.FromSeconds(120);

        public TimeSpan ActorRetirementPeriod { get; private set; } = TimeSpan.FromMinutes(60);

        public TimeSpan InactivityScanPeriod { get; private set; } = TimeSpan.FromMinutes(5);

        public IList<ActorTypeRegistration> Registration { get; private set; } = new List<ActorTypeRegistration>();

        public ActorManagerBuilder Set(IActorRepository repository)
        {
            ActorRepository = repository;
            return this;
        }

        public ActorManagerBuilder Register<T>(Func<IWorkContext, ActorKey, IActorManager, T> createImplementation) where T : IActor
        {
            Verify.IsNotNull(nameof(createImplementation), createImplementation);

            Registration.Add(new ActorTypeRegistration(typeof(T), (c, x, m) => createImplementation(c, x, m)));
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
                Capacity = Capacity,
                ActorRepository = ActorRepository,
                //Container = Container,
                ActorCallTimeout = ActorCallTimeout,
                ActorRetirementPeriod = ActorRetirementPeriod,
                InactivityScanPeriod = InactivityScanPeriod,
                Registration = new List<ActorTypeRegistration>(Registration),
            };

            return new ActorManager(configuration);
        }
    }
}
