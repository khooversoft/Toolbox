// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Actor;
using Khooversoft.Security;
using Khooversoft.Toolbox;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public class CertificateActorRepository : ICertificateRepository
    {
        private readonly IActorManager _actorManger;

        public CertificateActorRepository(IActorManager actorManager)
        {
            Verify.IsNotNull(nameof(actorManager), actorManager);

            _actorManger = actorManager;
        }

        public async Task<byte[]> Decrypt(IWorkContext context, LocalCertificateKey certificateKey, byte[] data)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(certificateKey), certificateKey);
            Verify.IsNotNull(nameof(data), data);

            ICertificateActor actor = await _actorManger.CreateProxyAsync<ICertificateActor>(context, certificateKey.CreateActorKey());
            return await actor.Decrypt(context, data);
        }

        public async Task<byte[]> Encrypt(IWorkContext context, LocalCertificateKey certificateKey, byte[] data)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(certificateKey), certificateKey);
            Verify.IsNotNull(nameof(data), data);

            ICertificateActor actor = await _actorManger.CreateProxyAsync<ICertificateActor>(context, certificateKey.CreateActorKey());
            return await actor.Encrypt(context, data);
        }

        public async Task<X509Certificate2> GetCertificate(IWorkContext context, LocalCertificateKey certificateKey, bool throwOnNotFound)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(certificateKey), certificateKey);

            ICertificateActor actor = await _actorManger.CreateProxyAsync<ICertificateActor>(context, certificateKey.CreateActorKey());
            return await actor.GetCertificate(context, throwOnNotFound);
        }
    }
}
