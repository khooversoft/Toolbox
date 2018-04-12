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
    /// Measure's the time between construction and dispose.  Use with a "using(...)" statement.
    /// </summary>
    public class TimerScope : IDisposable
    {
        private int _disposed = 0;
        private readonly Action<long> _stop;

        public TimerScope(Action start, Action<long> stop)
        {
            Verify.IsNotNull(nameof(start), start);
            Verify.IsNotNull(nameof(stop), stop);

            _stop = stop;
            Stopwatch = Stopwatch.StartNew();

            start();
        }

        public Stopwatch Stopwatch { get; }

        public void Dispose()
        {
            int readDisposed = Interlocked.CompareExchange(ref _disposed, 1, 0);
            if (readDisposed == 0)
            {
                _stop(Stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
