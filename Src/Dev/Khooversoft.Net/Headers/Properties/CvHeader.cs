// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;

namespace Khooversoft.Net
{
    public class CvHeader : IHttpHeaderProperty
    {
        public CvHeader(string value)
        {
            Verify.IsNotEmpty(nameof(value), value);

            Value = value;
        }

        public CvHeader(string[] values)
        {
            Verify.IsNotNull(nameof(values), values);
            Verify.Assert(values.Length > 0, nameof(values));

            Value = values[0];
        }

        public static string HeaderKey { get; } = "API-Cv";

        public string Key { get; } = CvHeader.HeaderKey;

        public string Value { get; }

        public string FormatValueForHttp()
        {
            return Value;
        }
    }
}
