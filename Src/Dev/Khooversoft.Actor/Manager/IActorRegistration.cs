using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Actor
{
    public interface IActorRegistration : IDisposable
    {
        Type ActorType { get; }

        ActorKey ActorKey { get; }

        IActorBase Instance { get; }

        T GetInstance<T>() where T : IActor;
    }
}
