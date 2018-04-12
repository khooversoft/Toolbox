// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;

namespace Khooversoft.Net
{
    public class TestDumpHeader : IHttpHeaderProperty
    {
        public enum Commands
        {
            Request,
            Reponse
        };

        public TestDumpHeader(Commands value)
        {
            Value = value;
        }

        public TestDumpHeader(string[] values)
        {
            Verify.IsNotNull(nameof(values), values);
            Verify.Assert(values.Length > 0, nameof(values));

            Value = (Commands)Enum.Parse(typeof(Commands), values[0], true);
        }

        public static string HeaderKey { get; } = "API-TestDump";

        public string Key { get; } = HeaderKey;

        public Commands Value { get; }

        public string FormatValueForHttp()
        {
            return Value.ToString();
        }
    }
}
