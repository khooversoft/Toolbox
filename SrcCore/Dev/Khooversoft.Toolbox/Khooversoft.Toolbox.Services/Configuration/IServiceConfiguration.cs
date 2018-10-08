// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using System.Collections.Generic;

namespace Khooversoft.Toolbox
{
    public interface IServiceConfiguration
    {
        IReadOnlyDictionary<string, object> Properties { get; }

        ILifetimeScope Container { get; }
    }
}
