// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Observers
{
    public abstract class SubjectBase<T> : ObservableBase<T>, IObserver<T>
    {
        public void OnCompleted()
        {
            this.OnCompletedCore();
        }

        protected virtual void OnCompletedCore()
        {
            _subject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this.OnErrorCore(error);
        }

        protected virtual void OnErrorCore(Exception error)
        {
            _subject.OnError(error);
        }

        public void OnNext(T value)
        {
            this.OnNextCore(value);
        }

        protected virtual void OnNextCore(T value)
        {
            _subject.OnNext(value);
        }
    }
}
