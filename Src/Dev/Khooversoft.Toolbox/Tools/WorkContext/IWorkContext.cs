using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Execution context
    /// </summary>
    public interface IWorkContext
    {
        CorrelationVector Cv { get; }
        Tag Tag { get; }
        ILifetimeScope Container { get; }
        IReadOnlyDictionary<string, object> Properties { get; }
        CancellationToken CancellationToken { get; }

        IWorkContext With(string key, object value);
        IWorkContext With<T>(T value) where T : class;
        IWorkContext WithExtended();
        IWorkContext WithIncrement();
        IWorkContext WithTag(Tag tag, [CallerMemberName] string memberName = null);
        IWorkContext WithMethodName([CallerMemberName] string memberName = null);

        WorkContextBuilder ToBuilder();
    }
}
