// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class VerifyExtensions
    {
        /// <summary>
        /// Test if object is not null and if so, pass it through
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="self">value</param>
        /// <returns>value</returns>
        /// <exception cref="ArgumentNullException">if null</exception>
        public static T AssertNotNull<T>(this T self, string name = null) where T : class
        {
            if (self == null)
            {
                name = name ?? nameof(self);
                throw new ArgumentNullException(name);
            }

            return self;
        }

        /// <summary>
        /// Test if object is not null and if so, pass it through
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="self">value</param>
        /// <returns>value</returns>
        /// <exception cref="ArgumentNullException">if null</exception>
        public static string AssertNotEmpty(this string self, string name = null)
        {
            if (self?.IsEmpty() == true)
            {
                name = name ?? nameof(self);
                throw new ArgumentException("String is not empty", name);
            }

            return self;
        }
    }
}
