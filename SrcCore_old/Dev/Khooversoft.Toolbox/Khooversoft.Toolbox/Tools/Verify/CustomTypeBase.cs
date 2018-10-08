// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public abstract class CustomTypeBase<T> : ICustomType<T>
    {
        public CustomTypeBase(T value)
        {
            Verify.IsNotNull(nameof(value), value);

            Value = value;
        }

        public T Value { get; }

        public object GetObjectValue() { return Value; }

        public virtual bool IsValueValid()
        {
            if (EqualityComparer<T>.Default.Equals(Value, default(T)))
            {
                return false;
            }

            return true;
        }
    }
}
