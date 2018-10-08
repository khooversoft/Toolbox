// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Actor;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Services
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
