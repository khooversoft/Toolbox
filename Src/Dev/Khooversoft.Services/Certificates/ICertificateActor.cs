// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Actor;
using Khooversoft.Toolbox;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Khooversoft.Services
{
    public interface ICertificateActor : IActor
    {
        Task<X509Certificate2> GetCertificate(IWorkContext context, bool throwOnNotFound);

        Task<byte[]> Encrypt(IWorkContext context, byte[] data);

        Task<byte[]> Decrypt(IWorkContext context, byte[] data);
    }
}
