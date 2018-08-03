// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Actor
{
    /// <summary>
    /// Actor key, a GUID is created from the string vector
    /// </summary>
    public class ActorKey
    {
        /// <summary>
        /// Construct actor key from vector
        /// </summary>
        /// <param name="vectorKey"></param>
        public ActorKey(string vectorKey)
        {
            Verify.IsNotEmpty(nameof(vectorKey), vectorKey);

            VectorKey = vectorKey;
            Key = VectorKey.ToLower().ToGuid();
        }

        /// <summary>
        /// Actor key (hash from vector key)
        /// </summary>
        public Guid Key { get; }

        /// <summary>
        /// Vector key
        /// </summary>
        public string VectorKey { get; }

        public override string ToString()
        {
            return $"Key={Key}, Vector key={VectorKey}";
        }
    }
}
