// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Actor registration for lambda activator
    /// </summary>
    public class ActorTypeRegistration
    {
        private readonly Tag _tag = new Tag(nameof(ActorTypeRegistration));

        public ActorTypeRegistration(Type interfaceType, Func<IWorkContext, ActorKey, IActorManager, IActor> createImplementation)
        {
            Verify.IsNotNull(nameof(interfaceType), interfaceType);
            Verify.IsNotNull(nameof(createImplementation), createImplementation);

            InterfaceType = interfaceType;
            CreateImplementation = createImplementation;
        }

        /// <summary>
        /// Interface type
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// Create implementation by lambda
        /// </summary>
        public Func<IWorkContext, ActorKey, IActorManager, IActor> CreateImplementation { get; }
    }
}
