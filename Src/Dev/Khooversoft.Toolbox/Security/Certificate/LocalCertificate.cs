// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Toolbox.Security
{
    public class LocalCertificate
    {
        private static readonly Tag _tag = new Tag(nameof(LocalCertificate));
        private readonly object _lock = new object();
        private readonly CacheObject<X509Certificate2> _cachedCertificate = new CacheObject<X509Certificate2>(TimeSpan.FromHours(1));

        public LocalCertificate(LocalCertificateKey key)
        {
            Verify.IsNotNull(nameof(key), key);

            LocalCertificateKey = key;
        }

        public LocalCertificate(StoreLocation storeLocation, StoreName storeName, string thumbprint, bool requirePrivateKey)
        {
            LocalCertificateKey = new LocalCertificateKey(storeLocation, storeName, thumbprint, requirePrivateKey);
        }

        public LocalCertificateKey LocalCertificateKey { get; }

        /// <summary>
        /// find certificate by thumbprint
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="context">work context</param>
        /// <param name="throwOnNotFound">if true, throw exception if not found</param>
        /// <exception cref="ProgramExitException">Certificate is not found</exception>
        /// <returns>X509 certificate</returns>
        public X509Certificate2 GetCertificate(IWorkContext context, bool? throwOnNotFound = null)
        {
            context = context.WithTag(_tag);
            Exception saveException = null;
            X509Certificate2 certificate = null;

            throwOnNotFound = throwOnNotFound ?? LocalCertificateKey.RequirePrivateKey;

            lock (_lock)
            {
                if (_cachedCertificate.TryGetValue(out certificate))
                {
                    return certificate;
                }

                using (X509Store store = new X509Store(LocalCertificateKey.StoreName, LocalCertificateKey.StoreLocation))
                {
                    ToolboxEventSource.Log.Verbose(context, $"Looking for certificate for {this}");

                    try
                    {
                        store.Open(OpenFlags.ReadOnly);
                        X509Certificate2Collection certificateList = store.Certificates.Find(X509FindType.FindByThumbprint, LocalCertificateKey.Thumbprint, validOnly: false);

                        if (certificateList?.Count != 0)
                        {
                            certificate = certificateList
                                .OfType<X509Certificate2>()
                                .Where(x => !LocalCertificateKey.RequirePrivateKey || x.HasPrivateKey)
                                .FirstOrDefault();

                            _cachedCertificate.Set(certificate);
                        }
                    }
                    catch (Exception ex)
                    {
                        ToolboxEventSource.Log.Warning(context, $"Exception: {ex}");
                        _cachedCertificate.Clear();
                        saveException = ex;
                    }
                }

                ToolboxEventSource.Log.Verbose(context, $"{(_cachedCertificate != null ? "Found" : "Not found")} certificate for {this}");
                if (certificate == null && throwOnNotFound == true)
                {
                    throw new ArgumentException($"Cannot find certificate for {this}", saveException);
                }

                return certificate;
            }
        }

        public override string ToString()
        {
            return $"StoreName={LocalCertificateKey.StoreName}, StoreLocation={LocalCertificateKey.StoreLocation}, Thumbprint={LocalCertificateKey.Thumbprint}";
        }
    }
}
