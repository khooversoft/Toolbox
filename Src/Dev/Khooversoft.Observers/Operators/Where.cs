// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Khooversoft.Observers
{
    /// <summary>
    /// Where clause, ability to filter data streams
    /// </summary>
    /// <typeparam name="T">message type</typeparam>
    public class WhereObservable<T> : SingleObserverBase<T>
    {
        private readonly Func<T, bool> _predicate;

        /// <summary>
        /// Construct "Where" filter constructor on source of observable
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="predicate">filter lambda</param>
        public WhereObservable(IObservable<T> source, Func<T, bool> predicate)
        {
            _predicate = predicate;
            _sourceDisposable = source.Subscribe(this);
        }

        protected override void OnNextCore(T value)
        {
            bool shouldRun = false;

            try
            {
                shouldRun = _predicate(value);
            }
            catch (Exception exception)
            {
                _observer.OnError(exception);
                return;
            }

            if (shouldRun)
            {
                _observer.OnNext(value);
            }
        }
    }
}
