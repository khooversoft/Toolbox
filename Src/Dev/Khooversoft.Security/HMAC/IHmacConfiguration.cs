// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Khooversoft.Security
{
    public interface IHmacConfiguration
    {
        IEnumerable<string> Headers { get; }
    }
}
