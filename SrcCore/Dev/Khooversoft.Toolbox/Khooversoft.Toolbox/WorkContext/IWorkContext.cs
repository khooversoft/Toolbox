// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Execution context
    /// </summary>
    public interface IWorkContext
    {
        CorrelationVector Cv { get; }
        Tag Tag { get; }
        ILifetimeScope Container { get; }
        IPropertyBag Properties { get; }
        CancellationToken CancellationToken { get; }
        IEventLog EventLog { get; }
        IEventDimensions Dimensions { get; }

        IWorkContext With(string key, object value);
        IWorkContext With<T>(T value) where T : class;
        IWorkContext WithExtended();
        IWorkContext WithIncrement();
        IWorkContext WithTag(Tag tag, [CallerMemberName] string memberName = null);
        IWorkContext WithMethodName([CallerMemberName] string memberName = null);

        WorkContextBuilder ToBuilder();
    }
}
