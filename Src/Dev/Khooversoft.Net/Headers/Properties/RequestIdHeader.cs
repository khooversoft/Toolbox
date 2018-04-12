// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;

namespace Khooversoft.Net
{
    public class RequestIdHeader : IHttpHeaderProperty
    {
        public RequestIdHeader()
        {
            Value = Guid.NewGuid().ToString();
        }

        public RequestIdHeader(string value)
        {
            Verify.IsNotEmpty(nameof(value), value);
            Value = value;
        }

        public RequestIdHeader(string[] values)
        {
            Verify.IsNotNull(nameof(values), values);
            Verify.Assert(values.Length > 0, nameof(values));

            Value = values[0];
        }

        public static string HeaderKey { get; } = "API-RequestId";

        public string Key { get; } = HeaderKey;

        public string Value { get; }

        public string FormatValueForHttp()
        {
            return Value;
        }
    }
}
