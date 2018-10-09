// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using Khooversoft.Toolbox.Actor;

namespace Khooversoft.Toolbox.Services
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

        public static ActorConfigurationBuilder AddIdentityModule(this ActorConfigurationBuilder self, ILifetimeScope container)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(container), container);

            self.Register<IIdentityActor>((c, k, m) => container.Resolve<IIdentityActor>(new TypedParameter(typeof(ActorKey), k), new TypedParameter(typeof(IActorManager), m)));

            return self;
        }
    }
}
