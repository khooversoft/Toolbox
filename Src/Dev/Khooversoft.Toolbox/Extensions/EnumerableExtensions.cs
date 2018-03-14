using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class EnumerableExtensions
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
