// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Threading;

namespace Khooversoft.Observers
{
    public class ObservableContext<T> : IDisposable
    {
        private DisposableManager _disposableManager = new DisposableManager();
        private IObservable<T> _currentObservable;
        private IObservable<T> _source;

        public ObservableContext(IObservable<T> observable)
        {
            Verify.IsNotNull(nameof(observable), observable);

            _source = _currentObservable = observable;
        }

        public ObservableContext<T> SendTo(IObserver<T> observer)
        {
            Verify.IsNotNull(nameof(observer), observer);

            var dispoable = _currentObservable.Subscribe(observer);
            _disposableManager.Add(dispoable);

            if (observer is IObservable<T>)
            {
                _currentObservable = (IObservable<T>)observer;
            }

            return this;
        }

        public void Dispose()
        {
            var currentSource = Interlocked.Exchange(ref _source, null);
            if (currentSource != null && currentSource is IDisposable)
            {
                ((IDisposable)currentSource).Dispose();
            }

            var currentManager = Interlocked.Exchange(ref _disposableManager, null);
            currentManager?.Dispose();
        }
    }

    public static class ObservableContextExtensions
    {
        public static ObservableContext<T> SendTo<T>(this IObservable<T> observable, IObserver<T> observer)
        {
            Verify.IsNotNull(nameof(observable), observable);
            Verify.IsNotNull(nameof(observer), observer);

            return new ObservableContext<T>(observable)
                .SendTo(observer);
        }
    }
}
