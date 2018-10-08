// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System.Diagnostics;

namespace Khooversoft.Toolbox.Services
{
    /// <summary>
    /// Token key, format is "{authorizationIssuer}/{requestingSubject}"
    /// </summary>
    [DebuggerDisplay("Value={Value}")]
    public class TokenKey
    {
        private readonly StringVectorBind _bind;

        private TokenKey()
        {
            _bind = new StringVectorBind(2, 2)
                .Add(() => AuthorizationIssuer, x => AuthorizationIssuer = x)
                .Add(() => RequestingSubject, x => RequestingSubject = x);
        }

        public TokenKey(string value)
            : this()
        {
            Verify.IsNotEmpty(nameof(value), value);

            var sv = new StringVector(value);
            _bind.Set(sv);
        }

        public TokenKey(string authorizationIssuer, string requstingSubject)
            : this()
        {
            Verify.IsNotEmpty(nameof(authorizationIssuer), authorizationIssuer);
            Verify.IsNotEmpty(nameof(requstingSubject), requstingSubject);

            AuthorizationIssuer = authorizationIssuer;
            RequestingSubject = requstingSubject;
        }

        /// <summary>
        /// Authorization issuer
        /// </summary>
        public string AuthorizationIssuer { get; private set; }

        /// <summary>
        /// Requesting subject identifier
        /// </summary>
        public string RequestingSubject { get; private set; }

        /// <summary>
        /// Get vector value
        /// </summary>
        public string Value { get { return _bind.Get().Value; } }

        /// <summary>
        /// Token key as string vector to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source">source</param>
        public static implicit operator string(TokenKey source)
        {
            return source.Value;
        }
    }
}
