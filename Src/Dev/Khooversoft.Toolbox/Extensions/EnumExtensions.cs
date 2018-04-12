// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Parse enum from string
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="self">string value</param>
        /// <param name="ignoreCase">True to ignore case</param>
        /// <returns>Enum type</returns>
        public static TEnum Parse<TEnum>(this string self, bool ignoreCase = false) where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), self, ignoreCase);
        }

        /// <summary>
        /// Try and parse enum with default value if required
        /// </summary>
        /// <typeparam name="TEnum">enum type</typeparam>
        /// <param name="self">string value</param>
        /// <param name="defaultValue">default value if string is empty</param>
        /// <param name="ignoreCase">ignore string case</param>
        /// <returns>parsed or default value</returns>
        public static TEnum TryParseOrDefault<TEnum>(this string self, TEnum defaultValue, bool ignoreCase = false) where TEnum : struct
        {
            if (self.IsEmpty())
            {
                return defaultValue;
            }

            TEnum result;
            if (!Enum.TryParse(self, ignoreCase, out result))
            {
                return (TEnum)defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Convert enum from enum
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="self">enum value</param>
        /// <param name="ignoreCase">True to ignore case</param>
        /// <returns>Enum type</returns>
        public static TEnum ConvertTo<TEnum>(this System.Enum self, bool ignoreCase = false) where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), self.ToString(), ignoreCase);
        }
    }
}
