// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Constructs a disposable object that can be used to finalize a resource or operation.
    /// 
    /// Dispose operations are thread safe
    /// </summary>
    public sealed class Scope : IDisposable
    {
        private Action _action;

        public Scope(Action disposeAction)
        {
            Verify.IsNotNull(nameof(disposeAction), disposeAction);

            _action = disposeAction;
        }

        public void Dispose()
        {
            Action action = Interlocked.Exchange(ref _action, null);
            action?.Invoke();
        }
    }
}
