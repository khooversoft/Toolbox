using Autofac;
using Khooversoft.Toolbox;
using System;
using System.Threading.Tasks;

namespace Khooversoft.Actor
{
    public interface IActorManager : IDisposable
    {
        IActorManager Register<T, TBase>(IWorkContext context)
            where TBase : class, IActorBase
            where T : IActor;

        IActorManager Register<T>(IWorkContext context, Func<IWorkContext, ActorKey, IActorManager, T> createImplementation) where T : IActor;

        Task<T> CreateProxyAsync<T>(IWorkContext context, ActorKey actorKey) where T : IActor;

        Task<bool> DeactivateAsync<T>(IWorkContext context, ActorKey actorKey);

        Task DeactivateAllAsync(IWorkContext context);

        IActorConfiguration Configuration { get; }

        ILifetimeScope Container { get; }
    }
}
