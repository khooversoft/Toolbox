using Khooversoft.Toolbox;
using System.Diagnostics;

namespace Khooversoft.Observers
{
    public class DebugObserver<T> : ObserverBase<T>
    {
        private readonly IFormatter<T, string> _formatter;

        public DebugObserver(IFormatter<T, string> formatter)
        {
            Verify.IsNotNull(nameof(formatter), formatter);

            _formatter = formatter;
        }

        protected override void OnNextCore(T value)
        {
            Debug.WriteLine(_formatter.Format(value));
        }
    }
}
