// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;

namespace Khooversoft.Toolbox.Security
{
    public class HmacIdentity
    {
        public HmacIdentity(string credential)
        {
            Verify.IsNotEmpty(nameof(credential), credential);

            Credential = credential;
        }

        public string Credential { get; }
    }
}
