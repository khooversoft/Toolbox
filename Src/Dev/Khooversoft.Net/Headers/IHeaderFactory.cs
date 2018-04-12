// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Khooversoft.Net
{
    public interface IHeaderFactory
    {
        void Add(string key, Func<string[], IHttpHeaderProperty> factory);

        IHttpHeaderProperty Create(string key, string[] values);
    };
}
