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
    /// Disposable manager, used to handle tracking and disposing
    /// of objects that implement IDisposable
    /// </summary>
    public class DisposableManager : IDisposable
    {
        private readonly object _lock = new object();
        private List<IDisposable> _container = new List<IDisposable>();

        /// <summary>
        /// Construct manager
        /// </summary>
        public DisposableManager()
        {
        }

        /// <summary>
        /// Construct manager with disposables
        /// </summary>
        /// <param name="disposables">disposables</param>
        public DisposableManager(IEnumerable<IDisposable> disposables)
        {
            Verify.IsNotNull(nameof(disposables), disposables);

            _container.AddRange(disposables);
        }

        /// <summary>
        /// Count of items in counter
        /// </summary>
        public int Count { get { return _container.Count; } }

        /// <summary>
        /// Add disposable item.  If the container has already been disposed,
        /// the item is dispose by this method.
        /// </summary>
        /// <param name="item">disposable item</param>
        /// <returns>this</returns>
        public DisposableManager Add(IDisposable item)
        {
            Verify.IsNotNull(nameof(item), item);

            var shouldDispose = false;

            lock (_lock)
            {
                shouldDispose = _container == null;

                if (_container != null)
                {
                    _container.Add(item);
                }
            }

            if (shouldDispose)
            {
                item.Dispose();
            }

            return this;
        }

        /// <summary>
        /// Dispose of all disposables in container
        /// </summary>
        public void Dispose()
        {
            IEnumerable<IDisposable> currentDisposables = (IEnumerable<IDisposable>)Interlocked.Exchange(ref _container, null);
            currentDisposables = currentDisposables ?? default(IDisposable[]);

            foreach (var item in currentDisposables)
            {
                item.Dispose();
            }
        }

        /// <summary>
        /// Language optimization - overload for adding disposable
        /// </summary>
        /// <param name="c">disposable manager</param>
        /// <param name="disposable">disposable</param>
        /// <returns></returns>
        public static DisposableManager operator +(DisposableManager c, IDisposable disposable)
        {
            c.Add(disposable);
            return c;
        }
    }
}
