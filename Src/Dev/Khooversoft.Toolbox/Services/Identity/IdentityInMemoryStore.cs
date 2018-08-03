// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Services
{
    public class IdentityInMemoryStore : IIdentityStore
    {
        private readonly ConcurrentDictionary<string, IdentityPrincipal> _store = new ConcurrentDictionary<string, IdentityPrincipal>(StringComparer.OrdinalIgnoreCase);
        private readonly Guid _storeId = Guid.NewGuid();

        public IdentityInMemoryStore()
        {
        }

        public IdentityPrincipal Get(IWorkContext context, PrincipalId principalId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(principalId), principalId);

            if (!_store.TryGetValue(principalId, out IdentityPrincipal identityPrincipal))
            {
                return null;
            }

            return identityPrincipal;
        }

        public Task<IdentityPrincipal> GetAsync(IWorkContext context, PrincipalId principalId)
        {
            return Task.FromResult(Get(context, principalId));
        }

        public IdentityInMemoryStore Set(IWorkContext context, IdentityPrincipal identityPrincipal)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(identityPrincipal), identityPrincipal);
            Verify.IsNotEmpty(nameof(identityPrincipal.PrincipalId), identityPrincipal.PrincipalId);

            _store[identityPrincipal.PrincipalId] = identityPrincipal.Clone();
            return this;
        }

        public Task SetAsync(IWorkContext context, IdentityPrincipal identityPrincipal)
        {
            Set(context, identityPrincipal);
            return Task.FromResult(0);
        }

        public IdentityPrincipal Remove(IWorkContext context, PrincipalId principalId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(principalId), principalId);

            if (!_store.TryRemove(principalId, out IdentityPrincipal identityPrincipal))
            {
                return null;
            }

            return identityPrincipal.Clone();
        }

        public Task<IdentityPrincipal> RemoveAsync(IWorkContext context, PrincipalId principalId)
        {
            return Task.FromResult(Remove(context, principalId));
        }
    }
}
