// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class X509CertificateExtensions
    {
        /// <summary>
        /// Encrypt with certificate (must be RSA)
        /// </summary>
        /// <param name="self">local certificate</param>
        /// <param name="context">work context</param>
        /// <param name="data">data to encrypted</param>
        /// <returns>byte array</returns>
        public static byte[] Encrypt(this X509Certificate2 self, byte[] data)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(data), data);

            using (RSA rsa = self.GetRSAPublicKey())
            {
                // OAEP allows for multiple hashing algorithms, what was formerly just "OAEP" is
                // now OAEP-SHA1.
                return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
            }
        }

        /// <summary>
        /// Decrypt data with certificate (must be RSA)
        /// </summary>
        /// <param name="self">local certificate</param>
        /// <param name="data">encrypted data</param>
        /// <returns>unencrypted byte array</returns>
        public static byte[] Decrypt(this X509Certificate2 self, byte[] data)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(data), data);

            using (RSA rsa = self.GetRSAPrivateKey())
            {
                return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA1);
            }
        }

        /// <summary>
        /// Return if certificate is expired
        /// </summary>
        /// <param name="self">certificate</param>
        /// <returns>true if expired, false if not</returns>
        public static bool IsExpired(this X509Certificate2 self)
        {
            Verify.IsNotNull(nameof(self), self);

            return DateTime.Now > self.NotAfter;
        }
    }
}
