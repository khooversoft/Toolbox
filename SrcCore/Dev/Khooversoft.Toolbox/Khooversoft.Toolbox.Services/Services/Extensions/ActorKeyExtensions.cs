// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Actor;
using Khooversoft.Toolbox.Security;

namespace Khooversoft.Toolbox.Services
{
    public static class ActorKeyExtensions
    {
        public static ActorKey CreateActorKey(this LocalCertificateKey self)
        {
            return new ActorKey(self.ToString());
        }

        public static ActorKey CreateActorKey(this IdentityPrincipal self)
        {
            return new ActorKey(self.PrincipalId);
        }

        public static ActorKey CreateActorKey(this TokenKey self)
        {
            return new ActorKey(self.Value);
        }
    }
}
