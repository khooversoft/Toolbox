// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Actor;
using System;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Services
{
    /// <summary>
    /// Identity, store identify information about a client or subject
    /// ActorKey = "ClientId"
    /// </summary>
    public class IdentityActor : ActorBase, IIdentityActor
    {
        private IdentityPrincipal _principal;
        private IIdentityStore _identityRepository;

        public IdentityActor(ActorKey actorKey, IActorManager actorManager, IIdentityStore identityRepository)
            : base(actorKey, actorManager)
        {
            Verify.IsNotNull(nameof(identityRepository), identityRepository);

            _identityRepository = identityRepository;
        }

        public async Task<IdentityPrincipal> Get(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            if (_principal != null)
            {
                return _principal;
            }

            _principal = await _identityRepository.GetAsync(context, new PrincipalId(ActorKey.VectorKey));
            return _principal;
        }

        public async Task Set(IWorkContext context, IdentityPrincipal identityPrincipal)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.Assert(ActorKey.VectorKey.Equals(identityPrincipal.PrincipalId, StringComparison.OrdinalIgnoreCase), "Client identity does not match actor key");

            await _identityRepository.SetAsync(context, identityPrincipal);

            _principal = null;
        }

        public async Task<IdentityPrincipal> Remove(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);

            IdentityPrincipal client = await _identityRepository.RemoveAsync(context, new PrincipalId(ActorKey.VectorKey));

            _principal = null;
            return client;
        }
    }
}
