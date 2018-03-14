using Khooversoft.Actor;
using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public class IdentityActorRepository : IIdentityRepository
    {
        private readonly IActorManager _actorManger;

        public IdentityActorRepository(IActorManager actorManager)
        {
            Verify.IsNotNull(nameof(actorManager), actorManager);

            _actorManger = actorManager;
        }

        public async Task SetAsync(IWorkContext context, IdentityPrincipal principal)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(principal), principal);

            IIdentityActor actor = await _actorManger.CreateProxyAsync<IIdentityActor>(context, new ActorKey(principal.PrincipalId));
            await actor.Set(context, principal);
        }

        public async Task<IdentityPrincipal> GetAsync(IWorkContext context, PrincipalId principalId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(principalId), principalId);

            IIdentityActor actor = await _actorManger.CreateProxyAsync<IIdentityActor>(context, new ActorKey(principalId));
            return await actor.Get(context);
        }

        public async Task<IdentityPrincipal> RemoveAsync(IWorkContext context, PrincipalId principalId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(principalId), principalId);

            IIdentityActor actor = await _actorManger.CreateProxyAsync<IIdentityActor>(context, new ActorKey(principalId));
            return await actor.Remove(context);
        }
    }
}
