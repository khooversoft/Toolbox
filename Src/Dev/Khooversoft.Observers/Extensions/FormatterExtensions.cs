// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Observers
{
    public static class FormatterExtensions
    {
        public class StringFormatter : IFormatter<string, string>
        {
            public string Format(string obj)
            {
                return obj;
            }
        }

        public static IFormatter<string, string> String { get; } = new StringFormatter();
    }
}
