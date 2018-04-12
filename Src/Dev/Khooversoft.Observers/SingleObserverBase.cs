// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;

namespace Khooversoft.Observers
{
    public abstract class SingleObserverBase<T> : ObserverBase<T>, IObservable<T>
    {
        protected IObserver<T> _observer;
        protected IDisposable _sourceDisposable;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Verify.IsNotNull(nameof(observer), observer);
            Verify.Assert<InvalidOperationException>(_observer == null, "Subscription already exists");

            _observer = observer;
            return _sourceDisposable;
        }

        protected override void OnCompletedCore()
        {
            Verify.Assert<InvalidOperationException>(_observer != null, "Subscription not set");
            _observer.OnCompleted();
        }

        protected override void OnErrorCore(Exception error)
        {
            Verify.Assert<InvalidOperationException>(_observer != null, "Subscription not set");
            _observer.OnError(error);
        }
    }
}
