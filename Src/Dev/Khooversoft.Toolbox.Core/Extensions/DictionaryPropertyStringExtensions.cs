// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class DictionaryPropertyStringExtensions
    {
        /// <summary>
        /// Convert dictionary to property string.  Format is "key1=value1[;key1=value1]..."
        /// </summary>
        /// <param name="self">dictionary</param>
        /// <param name="delimiter">delimiter to use (default = ';')</param>
        /// <returns>property string</returns>
        public static string ToPropertyString(this IDictionary<string, string> self, string delimiter = ";")
        {
            Verify.IsNotNull(nameof(self), self);

            return string.Join(delimiter, self.Select(x => $"{x.Key}={x.Value}"));
        }

        /// <summary>
        /// Parse property string to dictionary.  Format is "key1=value1[;key1=value1]..."
        /// </summary>
        /// <param name="self">property string</param>
        /// <param name="delimiter">delimiter to use (default = ';')</param>
        /// <returns>dictionary</returns>
        public static IDictionary<string, string> Parse(this string self, string delimiter = ";")
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (self.IsEmpty())
            {
                return dict;
            }

            foreach (var field in self.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = field.Split('=');
                Verify.Assert(parts.Length == 2, "Invalid format");

                dict.Add(parts[0].Trim(), parts[1].Trim());
            }

            return dict;
        }
    }
}
