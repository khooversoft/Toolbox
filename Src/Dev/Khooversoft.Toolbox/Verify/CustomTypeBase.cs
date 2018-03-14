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
            if (Value == null)
            {
                return false;
            }

            return true;
        }
    }
}
