// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class StringExtensions
    {
        /// <summary>
        /// Is string empty (null or white space)
        /// </summary>
        /// <param name="self">value</param>
        /// <returns>true or false</returns>
        public static bool IsEmpty(this string self)
        {
            return string.IsNullOrWhiteSpace(self);
        }

        /// <summary>
        /// Is string empty (null or white space)
        /// </summary>
        /// <param name="self">value</param>
        /// <returns>true or false</returns>
        public static bool IsNotEmpty(this string self)
        {
            return !string.IsNullOrWhiteSpace(self);
        }

        /// <summary>
        /// Safe sub string, return empty string or sub string from offset 0, if possible
        /// </summary>
        /// <param name="self">self</param>
        /// <param name="size">Number of characters to return</param>
        /// <returns>empty string or sub string if possible</returns>
        public static string SafeSubstring(this string self, int size)
        {
            return self.SafeSubstring(0, size);
        }

        /// <summary>
        /// Safe sub string, return empty string or sub string if possible
        /// </summary>
        /// <param name="self">self</param>
        /// <param name="start">starting position (zero offset)</param>
        /// <param name="size">Number of characters to return</param>
        /// <returns>empty string or sub string if possible</returns>
        public static string SafeSubstring(this string self, int start, int size)
        {
            if (self == null)
            {
                return string.Empty;
            }

            if (size == 0 || start >= self.Length)
            {
                return string.Empty;
            }

            return self.Substring(start, Math.Min(self.Length - start, size));
        }

        /// <summary>
        /// Convert string to guid
        /// </summary>
        /// <param name="self">string to convert, empty string will return empty guid</param>
        /// <returns>guid or empty guid</returns>
        public static Guid ToGuid(this string self)
        {
            if (self.IsEmpty())
            {
                return Guid.Empty;
            }

            using (var md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(self));
                return new Guid(data);
            }
        }

        /// <summary>
        /// Aggregate collection of string into a single string with a separator used between
        /// each value
        /// </summary>
        /// <param name="self">reference</param>
        /// <param name="separator">separator </param>
        /// <returns>string</returns>
        public static string Aggregate(this IEnumerable<string> self, string separator)
        {
            return string.Join(separator, self);
        }

        /// <summary>
        /// Safe convert string to value type
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self"></param>
        /// <param name="defaultValue"></param>
        /// <exception cref="ArgumentException">if type is not supported</exception>
        /// <returns>value parsed or default</returns>
        public static T SafeParse<T>(this string self, T defaultValue = default(T)) where T : struct
        {
            if (self.IsNotEmpty())
            {
                switch (defaultValue)
                {
                    case int v:
                        if (int.TryParse(self, out int intValue))
                        {
                            return (T)Convert.ChangeType(intValue, typeof(T));
                        }
                        break;

                    case long v:
                        if (long.TryParse(self, out long longValue))
                        {
                            return (T)Convert.ChangeType(longValue, typeof(T));
                        }
                        break;

                    case Guid v:
                        if (Guid.TryParse(self, out Guid guidValue))
                        {
                            return (T)Convert.ChangeType(guidValue, typeof(T));
                        }
                        break;

                    default:
                        throw new ArgumentException($"Unsupported type: {typeof(T).Name}");
                }
            }

            return defaultValue;
        }
    }
}
