// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
