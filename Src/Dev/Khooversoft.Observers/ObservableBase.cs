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
