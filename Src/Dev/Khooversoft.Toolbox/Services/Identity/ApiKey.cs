// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Cryptography;

namespace Khooversoft.Toolbox.Services
{
    public class ApiKey : StringType
    {
        public ApiKey(string value)
            : base(value, 100)
        {
        }

        public ApiKey(string value, DateTime? expires)
            : this(value)
        {
            Expires = expires;
        }

        public DateTime? Expires { get; }

        /// <summary>
        /// Has Api key expired
        /// </summary>
        public bool HasExpired { get { return Expires != null && DateTime.UtcNow > Expires; } }

        public override string ToString()
        {
            return Value;

        }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source">source reference</param>
        public static implicit operator string(ApiKey source)
        {
            return source?.Value;
        }

        /// <summary>
        /// Create web key using cryptography
        /// </summary>
        /// <returns>key</returns>
        public static ApiKey CreateApiKey(DateTime? expires)
        {
            using (var crypt = Rijndael.Create())
            {
                crypt.GenerateKey();
                return new ApiKey(crypt.Key.ToBase64String(), expires);
            }
        }
    }
}
