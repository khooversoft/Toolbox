using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Constructs a disposable object for finalization of object or operation
    /// </summary>
    /// <typeparam name="T">value type</typeparam>
    public sealed class Scope<T> : IDisposable
    {
        private Action<T> _action;

        public Scope(T value, Action<T> disposeAction)
        {
            Verify.IsNotNull(nameof(disposeAction), disposeAction);

            _action = disposeAction;
            this.Value = value;
        }

        public T Value { get; }

        public void Dispose()
        {
            Action<T> action = Interlocked.Exchange(ref _action, null);
            action?.Invoke(this.Value);
        }
    }
}
