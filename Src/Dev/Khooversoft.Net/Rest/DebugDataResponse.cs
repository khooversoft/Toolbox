// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Khooversoft.Net
{
    public class DebugDataResponse<T>
    {
        public T Value { get; set; }

        public DebugEventContractV1 DebugEvents { get; set; }
    }
}
