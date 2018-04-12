// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;

namespace Khooversoft.Net
{
    public interface IRestClientConfiguration
    {
        HttpClient HttpClient { get; }

        IReadOnlyDictionary<string, object> Properties { get; }

        IRestClientConfiguration WithProperty<T>(T value) where T : class;
    }
}
