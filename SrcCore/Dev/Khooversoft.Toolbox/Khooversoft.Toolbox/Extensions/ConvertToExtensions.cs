using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Khooversoft.Toolbox
{
    public static class ConvertToExtensions
    {

        /// <summary>
        /// Return list of public properties for object (anonymous)
        /// </summary>
        /// <typeparam name="T">type of self</typeparam>
        /// <param name="self">self</param>
        /// <returns>List of key value pairs</returns>
        public static IEnumerable<KeyValuePair<string, object>> ToKeyValues<T>(this T self)
        {
            BindingFlags publicAttributes = BindingFlags.Public | BindingFlags.Instance;

            return self.GetType().GetProperties(publicAttributes)
                .Where(x => x.CanRead)
                .Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(self, null)))
                .ToList();
        }

        /// <summary>
        /// Covert collection to stack
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="values">list of values</param>
        /// <returns>stack</returns>
        public static Stack<T> ToStack<T>(this IEnumerable<T> values)
        {
            return new Stack<T>(values);
        }

        /// <summary>
        /// Materialize is a process where append-only operations are converted to a static set of values.
        /// The values are in the form of key value pair and list must be ordered in least to most significant.
        /// 
        /// The data transformation is...
        ///   1) Insert "index" value so that order can be used to pick the last update to a specific key.
        ///   2) Group by the key, this is to detect duplicates and use the "index" to pick the "most" value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="self"></param>
        /// <param name="ignoreCase">true to ignore case with string</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> Materialized<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self, bool ignoreCase = true)
        {
            if (self == null)
            {
                return null;
            }

            IEqualityComparer<TKey> comparer = ignoreCase && (typeof(TKey) == typeof(string)) ?
                (IEqualityComparer<TKey>)StringComparer.InvariantCultureIgnoreCase :
                EqualityComparer<TKey>.Default;

            return self
                .Select((x, i) => new { pair = x, index = i })
                .GroupBy(x => x.pair.Key, comparer)
                .Select(x => x.OrderByDescending(y => y.index).First())
                .Select(x => x.pair);
        }
    }
}
