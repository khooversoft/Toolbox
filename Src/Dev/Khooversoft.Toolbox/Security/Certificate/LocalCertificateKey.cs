// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Toolbox.Security
{
    /// <summary>
    /// Local certificate key for windows certificate store
    /// 
    /// Also provides certificate key string vector for actor or storage
    /// </summary>
    public class LocalCertificateKey
    {
        private readonly StringVectorBind _bind;

        private LocalCertificateKey()
        {
            _bind = new StringVectorBind(4, 4)
                .Add(() => StoreLocation.ToString(), x => StoreLocation = x.Parse<StoreLocation>(ignoreCase: true))
                .Add(() => StoreName.ToString(), x => StoreName = x.Parse<StoreName>(ignoreCase: true))
                .Add(() => Thumbprint, x => Thumbprint = x)
                .Add(() => RequirePrivateKey.ToString(), x => RequirePrivateKey = bool.Parse(x));
        }

        public LocalCertificateKey(StoreLocation storeLocation, StoreName storeName, string thumbprint, bool requirePrivateKey)
            : this()
        {
            Verify.IsNotEmpty(nameof(thumbprint), thumbprint);

            StoreLocation = storeLocation;
            StoreName = storeName;
            Thumbprint = thumbprint;
            RequirePrivateKey = requirePrivateKey;
        }

        public LocalCertificateKey(string vectorKey)
            : this()
        {
            Verify.IsNotEmpty(nameof(vectorKey), vectorKey);

            _bind.Set(vectorKey);
        }

        public StoreLocation StoreLocation { get; private set; }

        public StoreName StoreName { get; private set; }

        public string Thumbprint { get; private set; }

        public bool RequirePrivateKey { get; private set; }

        public override string ToString()
        {
            return _bind.Get().ToString();
        }
    }
}
