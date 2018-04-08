using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public interface IStateItem
    {
        string Name { get; }

        bool IgnoreError { get; }

        Task<bool> Test(IWorkContext context);

        Task<bool> Set(IWorkContext context);
    }
}
