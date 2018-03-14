using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Actor
{
    public interface IActorBase : IDisposable
    {
        ActorKey ActorKey { get; }

        Task ActivateAsync(IWorkContext context);

        Task DeactivateAsync(IWorkContext context);
    }
}
