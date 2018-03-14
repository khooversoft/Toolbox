using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Khooversoft.Observers
{
    /// <summary>
    /// Represents an object that is both an observable sequence as well as an observer.
    /// Each notification is broad casted to all subscribed observers.
    /// </summary>
    /// <typeparam name="T">The type of the elements processed by the subject.</typeparam>
    public sealed class Subject<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private bool _isDisposed;
        private bool _isStopped;
        private ImmutableList<IObserver<T>> _observers = ImmutableList<IObserver<T>>.Empty;
        private object _lock = new object();
        private Exception _exception;

        /// <summary>
        /// Creates a subject.
        /// </summary>
        public Subject()
        {
        }

        /// <summary>
        /// Notifies all subscribed observers about the end of the sequence.
        /// </summary>
        public void OnCompleted()
        {
            var observerList = default(IEnumerable<IObserver<T>>);

            lock (_lock)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    observerList = new List<IObserver<T>>(_observers);
                    _observers = ImmutableList<IObserver<T>>.Empty;
                    _isStopped = true;
                }
            }

            if (observerList != null)
            {
                foreach (var o in observerList)
                {
                    o.OnCompleted();
                }
            }
        }

        /// <summary>
        /// Notifies all subscribed observers with the exception.
        /// </summary>
        /// <param name="error">The exception to send to all subscribed observers.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
        public void OnError(Exception error)
        {
            Verify.IsNotNull(nameof(error), error);

            var observerList = default(IEnumerable<IObserver<T>>);

            lock (_lock)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    observerList = new List<IObserver<T>>(_observers);
                    _observers = ImmutableList<IObserver<T>>.Empty;
                    _isStopped = true;
                    _exception = error;
                }
            }

            if (observerList != null)
            {
                foreach (var o in observerList)
                {
                    o.OnError(error);
                }
            }
        }

        /// <summary>
        /// Notifies all subscribed observers with the value.
        /// </summary>
        /// <param name="value">The value to send to all subscribed observers.</param>
        public void OnNext(T value)
        {
            var observerList = default(IEnumerable<IObserver<T>>);

            lock (_lock)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    observerList = new List<IObserver<T>>(_observers);
                }
            }

            if (observerList != null)
            {
                foreach (var o in observerList)
                {
                    o.OnNext(value);
                }
            }
        }

        /// <summary>
        /// Subscribes an observer to the subject.
        /// </summary>
        /// <param name="observer">Observer to subscribe to the subject.</param>
        /// <remarks>IDisposable object that can be used to unsubscribe the observer from the subject.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="observer"/> is null.</exception>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            Verify.IsNotNull(nameof(observer), observer);

            lock (_lock)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    _observers = _observers.Add(observer);
                    return new Subscription(this, observer);
                }
                else if (_exception != null)
                {
                    observer.OnError(_exception);
                    return EmptyDisposable.Empty;
                }
                else
                {
                    observer.OnCompleted();
                    return EmptyDisposable.Empty;
                }
            }
        }

        private void Unsubscribe(IObserver<T> observer)
        {
            lock (_lock)
            {
                if (_observers != null)
                {
                    _observers = _observers.Remove(observer);
                }
            }
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(string.Empty);
            }
        }

        /// <summary>
        /// Unsubscribe all observers and release resources.
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                _isDisposed = true;
                _observers = null;
            }
        }

        private class Subscription : IDisposable
        {
            Subject<T> _subject;
            IObserver<T> _observer;

            public Subscription(Subject<T> subject, IObserver<T> observer)
            {
                this._subject = subject;
                this._observer = observer;
            }

            public void Dispose()
            {
                var currentObserver = Interlocked.Exchange<IObserver<T>>(ref _observer, null);
                if (currentObserver != null)
                {
                    _subject.Unsubscribe(currentObserver);
                    _subject = null;
                }
            }
        }

        private sealed class EmptyDisposable : IDisposable
        {
            /// <summary>
            /// Singleton default disposable.
            /// </summary>
            public static readonly EmptyDisposable Empty = new EmptyDisposable();

            private EmptyDisposable()
            {
            }

            /// <summary>
            /// Does nothing.
            /// </summary>
            public void Dispose()
            {
                // no op
            }
        }
    }
}
