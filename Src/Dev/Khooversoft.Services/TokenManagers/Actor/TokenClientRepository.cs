using Khooversoft.Actor;
using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public class TokenClientRepository : ITokenClientRepository
    {
        private readonly IActorManager _actorManger;

        public TokenClientRepository(IActorManager actorManager)
        {
            Verify.IsNotNull(nameof(actorManager), actorManager);

            _actorManger = actorManager;
        }

        public async Task SetConfiguration(IWorkContext context, IClientTokenManagerConfiguration clientTokenManagerConfiguration)
        {
            Verify.IsNotNull(nameof(clientTokenManagerConfiguration), clientTokenManagerConfiguration);

            ITokenClientActor actor = await _actorManger.CreateProxyAsync<ITokenClientActor>(context, clientTokenManagerConfiguration.TokenKey.CreateActorKey());
            await actor.SetConfiguration(context, clientTokenManagerConfiguration);
        }

        public async Task<string> GetApiKey(IWorkContext context, TokenKey tokenKey)
        {
            Verify.IsNotNull(nameof(tokenKey), tokenKey);

            ITokenClientActor actor = await _actorManger.CreateProxyAsync<ITokenClientActor>(context, tokenKey.CreateActorKey());
            return await actor.GetApiKey(context);
        }
    }
}
