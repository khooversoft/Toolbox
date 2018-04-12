// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Scope class to record activity and timing using disposable pattern
    /// 
    /// Recommendation: Used in a "using(...)" statement
    /// </summary>
    public class ActivityScope : IDisposable
    {
        private int _disposed = 0;

        /// <summary>
        /// Construct scope
        /// </summary>
        /// <param name="context">work context</param>
        /// <param name="message">message (optional)</param>
        /// <param name="eventLog">event log (optional, default is toolbox's event source</param>
        public ActivityScope(IWorkContext context, string message = null, IEventLog eventLog = null)
        {
            Verify.IsNotNull(nameof(context), context);

            Context = context;
            EventLog = eventLog ?? ToolboxEventSource.Log;
            Message = message;
            Stopwatch = Stopwatch.StartNew();

            EventLog.ActivityStart(Context, Message);
        }

        public IWorkContext Context { get; }

        public IEventLog EventLog { get; }

        public string Message { get; }

        public Stopwatch Stopwatch { get; }

        public void Dispose()
        {
            int readDisposed = Interlocked.CompareExchange(ref _disposed, 1, 0);
            if (readDisposed == 0)
            {
                EventLog.ActivityStop(Context, Message, Stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
