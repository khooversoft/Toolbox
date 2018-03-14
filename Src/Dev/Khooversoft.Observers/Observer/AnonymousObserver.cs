using Khooversoft.Toolbox;
using System;

namespace Khooversoft.Observers
{
    /// <summary>
    /// Class to create an IObserver instance from delegate-based implementations of the On* methods.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    public sealed class AnonymousObserver<T> : ObserverBase<T>
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;
        private static readonly Action _nopAction = () => { };
        private static readonly Action<Exception> _throwAction = ex => { throw ex; };

        /// <summary>
        /// Creates an observer from the specified OnNext, OnError, and OnCompleted actions.
        /// </summary>
        /// <param name="onNext">Observer's OnNext action implementation.</param>
        /// <param name="onError">Observer's OnError action implementation.</param>
        /// <param name="onCompleted">Observer's OnCompleted action implementation.</param>
        public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            Verify.IsNotNull(nameof(onNext), onNext);
            Verify.IsNotNull(nameof(onError), onError);
            Verify.IsNotNull(nameof(onCompleted), onCompleted);

            _onNext = onNext;
            _onError = onError;
            _onCompleted = onCompleted;
        }

        /// <summary>
        /// Creates an observer from the specified OnNext action.
        /// </summary>
        /// <param name="onNext">Observer's OnNext action implementation.</param>
        public AnonymousObserver(Action<T> onNext)
            : this(onNext, _throwAction, _nopAction)
        {
        }

        /// <summary>
        /// Creates an observer from the specified OnNext and OnError actions.
        /// </summary>
        /// <param name="onNext">Observer's OnNext action implementation.</param>
        /// <param name="onError">Observer's OnError action implementation.</param>
        public AnonymousObserver(Action<T> onNext, Action<Exception> onError)
            : this(onNext, onError, _nopAction)
        {
        }

        /// <summary>
        /// Creates an observer from the specified OnNext and OnCompleted actions.
        /// </summary>
        /// <param name="onNext">Observer's OnNext action implementation.</param>
        /// <param name="onCompleted">Observer's OnCompleted action implementation.</param>
        public AnonymousObserver(Action<T> onNext, Action onCompleted)
            : this(onNext, _throwAction, onCompleted)
        {
        }

        /// <summary>
        /// Calls the onNext action.
        /// </summary>
        /// <param name="value">Next element in the sequence.</param>
        protected override void OnNextCore(T value)
        {
            _onNext(value);
        }

        /// <summary>
        /// Calls the onError action.
        /// </summary>
        /// <param name="error">The error that has occurred.</param>
        protected override void OnErrorCore(Exception error)
        {
            _onError(error);
        }

        /// <summary>
        /// Calls the onCompleted action.
        /// </summary>
        protected override void OnCompletedCore()
        {
            _onCompleted();
        }
    }
}
