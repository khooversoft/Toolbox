using Khooversoft.Toolbox;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Khooversoft.Security
{
    /// <summary>
    /// Build and encrypt data, creates immutable object, SecretData
    /// </summary>
    public class SecretDataBuilder
    {
        public SecretDataBuilder(bool allowNull = false)
        {
            AllowNull = allowNull;
        }

        public bool AllowNull { get; private set; }

        public X509Certificate2 Certificate { get; private set; }

        public string Value { get; private set; }

        public SecretDataBuilder SetAllowNull(bool allowNull)
        {
            AllowNull = allowNull;
            return this;
        }

        public SecretDataBuilder SetCertificate(X509Certificate2 certificate)
        {
            Verify.IsNotNull(nameof(certificate), certificate);

            Certificate = certificate;
            return this;
        }

        public SecretDataBuilder SetValue(string value)
        {
            Value = value;
            return this;
        }

        /// <summary>
        /// Create secret data by encrypting the value with the certificate,
        /// stored in an immutable object
        /// </summary>
        /// <returns>secret data</returns>
        public SecretData Encrypt()
        {
            Verify.Assert<InvalidOperationException>(Certificate != null, "Certificate has not been loaded");
            return new SecretData(Certificate, Value);
        }

        /// <summary>
        /// Convert base 64 data representing encoded data to secret data
        /// </summary>
        /// <returns>Secret value</returns>
        public SecretData Convert()
        {
            byte[] bytes = System.Convert.FromBase64String(Value);

            if (Certificate == null)
            {
                return new SecretData(bytes);
            }

            return new SecretData(Certificate, bytes);
        }
    }
}
