// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Khooversoft.Toolbox.Security
{
    /// <summary>
    /// Immutable class for storing encrypted data
    /// </summary>
    public class SecretData : ICustomType<byte[]>
    {
        /// <summary>
        /// Construct with secret with encrypted data as bytes
        /// </summary>
        /// <param name="encryptedData">byte array of encrypted data</param>
        public SecretData(byte[] encryptedData)
        {
            Verify.IsNotNull(nameof(encryptedData), encryptedData);

            Value = encryptedData;
        }

        /// <summary>
        /// Construct secret with encrypted data and certificate
        /// </summary>
        /// <param name="certificate">certificate</param>
        /// <param name="encryptedData">byte array of encrypted data</param>
        public SecretData(X509Certificate2 certificate, byte[] encryptedData)
        {
            Verify.IsNotNull(nameof(certificate), certificate);
            Verify.IsNotNull(nameof(encryptedData), encryptedData);

            Certificate = certificate;
            Value = encryptedData;
        }

        /// <summary>
        /// Construct and encrypt data
        /// </summary>
        /// <param name="certificate">certificate</param>
        /// <param name="unencryptedData">string to be encrypted</param>
        public SecretData(X509Certificate2 certificate, string unencryptedData)
        {
            Verify.IsNotNull(nameof(certificate), certificate);
            Verify.IsNotEmpty(nameof(unencryptedData), unencryptedData);

            Certificate = certificate;

            byte[] bytes = Encoding.UTF8.GetBytes(unencryptedData);
            Value = Certificate.Encrypt(bytes);
        }

        public X509Certificate2 Certificate { get; }

        public byte[] Value { get; }

        /// <summary>
        /// Create new instance with certificate
        /// </summary>
        /// <param name="certificate">certificate</param>
        /// <returns>new instance</returns>
        public SecretData WithCertificate(X509Certificate2 certificate)
        {
            Verify.IsNotNull(nameof(certificate), certificate);

            return new SecretData(certificate, Value);
        }

        /// <summary>
        /// Convert byte array to base 64 string, used for storage
        /// </summary>
        /// <returns>base 64 string</returns>
        public object GetObjectValue()
        {
            return ToStorage();
        }

        public bool IsValueValid()
        {
            return Value != null;
        }

        /// <summary>
        /// Convert encrypted data to base 64 string
        /// </summary>
        /// <returns>base 64 of encrypted data</returns>
        public string ToStorage()
        {
            if (Value == null)
            {
                return null;
            }

            return Convert.ToBase64String(Value);
        }

        /// <summary>
        /// Decrypt data to base 64 string (to client)
        /// </summary>
        /// <returns>base 64 of decrypted data</returns>
        public string Decrypt()
        {
            if (Value == null)
            {
                return null;
            }

            byte[] bytes = Certificate.Decrypt(Value);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
