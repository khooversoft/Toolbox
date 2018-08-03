// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Actor;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Services
{
    public class IdentityActorRepository : IIdentityRepository
    {
        private readonly IActorManager _actorManger;

        public IdentityActorRepository(IActorManager actorManager)
        {
            Verify.IsNotNull(nameof(actorManager), actorManager);

            _actorManger = actorManager;
        }

        public async Task SetAsync(IWorkContext context, IdentityPrincipal identityPrincipal)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(identityPrincipal), identityPrincipal);

            IIdentityActor actor = await _actorManger.CreateProxyAsync<IIdentityActor>(context, new ActorKey(identityPrincipal.PrincipalId));
            await actor.Set(context, identityPrincipal);
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
