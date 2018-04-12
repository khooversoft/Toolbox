// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Khooversoft.Observers
{
    public abstract class ObservableBase<T> : IObservable<T>
    {
        protected readonly Subject<T> _subject = new Subject<T>();

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}
