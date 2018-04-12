// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
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
    }
}
