// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using Khooversoft.Toolbox.Actor;

namespace Khooversoft.Toolbox.Services
{
    public class CertificateAutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CertificateActor>().As<ICertificateActor>();
            builder.RegisterType<CertificateActorRepository>().As<ICertificateRepository>().SingleInstance();
        }
    }

    public static class CertificateAutoFacModuleExtension
    {
        public static ContainerBuilder AddCertificateModule(this ContainerBuilder self)
        {
            self.RegisterModule(new CertificateAutoFacModule());
            return self;
        }

        public static ActorManagerBuilder AddCertificateModule(this ActorManagerBuilder self, ILifetimeScope container)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(container), container);

            self.Register<ICertificateActor>((c, k, m) => container.Resolve<ICertificateActor>(new TypedParameter(typeof(ActorKey), k), new TypedParameter(typeof(IActorManager), m)));

            return self;
        }
    }
}
