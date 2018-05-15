// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using Khooversoft.Actor;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public class IdentityActorAutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IdentityActor>().As<IIdentityActor>();
            builder.RegisterType<IdentityInMemoryStore>().As<IIdentityStore>().SingleInstance();
            builder.RegisterType<IdentityActorRepository>().As<IIdentityRepository>().SingleInstance();
        }
    }

    public static class IdentityActorAutoFacModuleExtension
    {
        public static ContainerBuilder AddIdentityModule(this ContainerBuilder self)
        {
            self.RegisterModule(new IdentityActorAutoFacModule());
            return self;
        }

        public static ActorManagerBuilder AddIdentityModule(this ActorManagerBuilder self, ILifetimeScope container)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(container), container);

            self.Register<IIdentityActor>((c, k, m) => container.Resolve<IIdentityActor>(new TypedParameter(typeof(ActorKey), k), new TypedParameter(typeof(IActorManager), m)));

            return self;
        }
    }
}
