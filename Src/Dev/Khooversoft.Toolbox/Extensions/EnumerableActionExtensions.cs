// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class EnumerableActionExtensions
    {
        /// <summary>
        /// Starts a enumerable sequence
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="value">value</param>
        /// <returns>enumerable of value</returns>
        public static IEnumerable<T> ToEnumerable<T>(this T value)
        {
            yield return value;
        }

        /// <summary>
        /// Apply action to each item in enumerable
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="list">list of types</param>
        /// <param name="action">action to be applied</param>
        public static void Run<T>(this IEnumerable<T> list, Action<T> action)
        {
            Verify.IsNotNull(nameof(list), list);
            Verify.IsNotNull(nameof(action), action);

            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// Run action <see langword="async"/>
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">enumerable</param>
        /// <param name="doAction">action</param>
        /// <returns></returns>
        public static async Task RunAsync<T>(this IEnumerable<T> self, Func<T, Task> doAction)
        {
            foreach (var item in self)
            {
                await doAction(item);
            }
        }

        /// <summary>
        /// Apply action to each item in enumerable
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="list">list of types</param>
        /// <param name="action">action to be applied (int is an index)</param>
        public static void Run<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            Verify.IsNotNull(nameof(list), list);
            Verify.IsNotNull(nameof(action), action);

            int index = 0;
            foreach (var item in list)
            {
                action(item, index++);
            }
        }

        /// <summary>
        /// Perform action on T, return T
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="list">enumerable of T</param>
        /// <param name="action">action</param>
        /// <returns>enumerable T</returns>
        public static IEnumerable<T> Do<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        /// Perform action on T, return T
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="list">enumerable of T</param>
        /// <param name="action">action</param>
        /// <returns>enumerable T</returns>
        public static IEnumerable<T> Do<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            int index = 0;
            foreach (var item in list)
            {
                action(item, index++);
                yield return item;
            }
        }
    }
}
