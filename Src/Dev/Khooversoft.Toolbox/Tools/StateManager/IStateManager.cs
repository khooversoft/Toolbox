using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public interface IStateManager
    {
        IReadOnlyList<IStateItem> StateItems { get; }

        bool IsRunning { get; }

        bool IsSuccessful { get; }

        StateContext Run(IWorkContext context);

        Task<StateContext> RunAsync(IWorkContext context);

        StateContext Test(IWorkContext context);

        Task<StateContext> TestAsync(IWorkContext context);
    }
}
