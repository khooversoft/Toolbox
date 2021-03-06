﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Service configuration, is immutable.  Changes impact full service stack
    /// where this interface is used.
    /// 
    /// Class is thread safe
    /// </summary>
    public class ServiceConfiguration : IServiceConfiguration
    {
        /// <summary>
        /// Properties (concurrent dictionary)
        /// </summary>
        public ConcurrentDictionary<string, object> Properties { get; } = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Auto fact container
        /// </summary>
        public ILifetimeScope Container { get; set; }

        /// <summary>
        /// Properties (read only from the interface)
        /// </summary>
        IReadOnlyDictionary<string, object> IServiceConfiguration.Properties => Properties;
    }
}
