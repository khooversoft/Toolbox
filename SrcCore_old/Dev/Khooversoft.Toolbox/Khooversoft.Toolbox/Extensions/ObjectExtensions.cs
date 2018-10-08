// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    [DebuggerStepThrough]
    public static class ObjectExtensions
    {
        /// <summary>
        /// Safe change type
        /// </summary>
        /// <typeparam name="T">type to change to</typeparam>
        /// <param name="self">value</param>
        /// <returns>converted type</returns>
        public static T AsTypeOrDefault<T>(this object self)
        {
            if (self is T)
            {
                return (T)Convert.ChangeType(self, typeof(T));
            }

            return default;
        }

        /// <summary>
        /// If logic in line
        /// </summary>
        /// <typeparam name="T">from type</typeparam>
        /// <typeparam name="TResult">to type</typeparam>
        /// <param name="self">value</param>
        /// <param name="test">test function</param>
        /// <param name="ifTrue">function if test is true</param>
        /// <param name="ifFalse">function if test is false</param>
        /// <returns></returns>
        public static TResult If<T, TResult>(this T self, Func<T, bool> test, Func<T, TResult> ifTrue, Func<T, TResult> ifFalse)
        {
            Verify.IsNotNull(nameof(ifTrue), ifTrue);
            Verify.IsNotNull(nameof(ifFalse), ifFalse);

            return test(self) ? ifTrue(self) : ifFalse(self);
        }

        /// <summary>
        /// Test to is if value is null
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="self">value</param>
        /// <returns>true if null, throws exception if not</returns>
        public static bool IsNull<T>(this T self) where T : class
        {
            return self == null;
        }

        /// <summary>
        /// Test if value is not null
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="self">value</param>
        /// <returns>true if not null</returns>
        public static bool IsNotNull<T>(this T self) where T : class
        {
            return self != null;
        }

        /// <summary>
        /// Test to is if value is null
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="self">value</param>
        /// <returns>true if null, throws exception if not</returns>
        public static bool IsNull<T>(this T? self) where T : struct
        {
            return self == null;
        }

        /// <summary>
        /// Test if value is not null
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="self">value</param>
        /// <returns>true if not null</returns>
        /// <exception cref="ArgumentNullException">if is null</exception>
        public static bool IsNotNull<T>(this T? self) where T : struct
        {
            return self != null;
        }

        /// <summary>
        /// Test if object is null and if so, run action on object, pass through
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="self">self</param>
        /// <param name="action">action to perform on failure</param>
        /// <returns>self</returns>
        public static T RunIfNull<T>(this T self, Action action)
            where T : class
        {
            Verify.IsNotNull(nameof(action), action);

            if (self == null)
            {
                action();
            }

            return self;
        }

        /// <summary>
        /// Test if object is not null and if so, run action on object, pass through
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="self">self</param>
        /// <param name="action">action to perform on failure</param>
        /// <returns>self</returns>
        public static T RunIfNotNull<T>(this T self, Action<T> action)
            where T : class
        {
            Verify.IsNotNull(nameof(action), action);

            if (self != null)
            {
                action(self);
            }

            return self;
        }
    }
}
