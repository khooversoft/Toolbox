// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Observers
{
    /// <summary>
    /// Standard sequence operators for IObservable
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// Filters the elements of an observable sequence based on a predicate.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">An observable sequence whose elements to filter.</param>
        /// <param name="predicate">A function to test each source element for a condition.</param>
        public static IObservable<T> Where<T>(this IObservable<T> source, Func<T, bool> predicate)
        {
            Verify.IsNotNull(nameof(source), source);
            Verify.IsNotNull(nameof(predicate), predicate);

            return new WhereObservable<T>(source, predicate);
        }

        /// <summary>
        /// Subscribes an element handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <returns>IDisposable object used to unsubscribe from the observable sequence.</returns>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            Verify.IsNotNull(nameof(source), source);
            Verify.IsNotNull(nameof(onNext), onNext);

            return source.Subscribe(new AnonymousObserver<T>(onNext));
        }

        /// <summary>
        /// Subscribes an element handler and an exception handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
        /// <returns>IDisposable object used to unsubscribe from the observable sequence.</returns>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            Verify.IsNotNull(nameof(source), source);
            Verify.IsNotNull(nameof(onNext), onNext);
            Verify.IsNotNull(nameof(onError), onError);

            return source.Subscribe(new AnonymousObserver<T>(onNext, onError));
        }

        /// <summary>
        /// Subscribes an element handler and a completion handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onCompleted">Action to invoke upon graceful termination of the observable sequence.</param>
        /// <returns>IDisposable object used to unsubscribe from the observable sequence.</returns>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            Verify.IsNotNull(nameof(source), source);
            Verify.IsNotNull(nameof(onNext), onNext);
            Verify.IsNotNull(nameof(onCompleted), onCompleted);

            return source.Subscribe(new AnonymousObserver<T>(onNext, onCompleted));
        }

        /// <summary>
        /// Subscribes an element handler, an exception handler, and a completion handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onError">Action to invoke upon exceptional termination of the observable sequence.</param>
        /// <param name="onCompleted">Action to invoke upon graceful termination of the observable sequence.</param>
        /// <returns>IDisposable object used to unsubscribe from the observable sequence.</returns>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            Verify.IsNotNull(nameof(source), source);
            Verify.IsNotNull(nameof(onNext), onNext);
            Verify.IsNotNull(nameof(onError), onError);
            Verify.IsNotNull(nameof(onCompleted), onCompleted);

            return source.Subscribe(new AnonymousObserver<T>(onNext, onError, onCompleted));
        }
    }
}
