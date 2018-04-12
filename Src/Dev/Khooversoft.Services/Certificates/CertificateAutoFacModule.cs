// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;

namespace Khooversoft.Services
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
    }
}
