using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Actor
{
    public interface IActorRepository
    {
        Task ClearAsync(IWorkContext context);

        Task SetAsync(IWorkContext context, IActorRegistration registration);

        IActorRegistration Lookup(Type actorType, ActorKey actorKey);

        Task<IActorRegistration> RemoveAsync(IWorkContext context, Type actorType, ActorKey actorKey);
    }
}
