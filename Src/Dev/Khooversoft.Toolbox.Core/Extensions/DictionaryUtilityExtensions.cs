// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class DictionaryUtilityExtensions
    {
        /// <summary>
        /// Add collection of values to dictionary
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="values">values</param>
        /// <returns>self</returns>
        public static IDictionary<TKey, TValue> Add<TKey, TValue>(this IDictionary<TKey, TValue> self, IEnumerable<KeyValuePair<TKey, TValue>> values)
            where TKey : class
            where TValue : class
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(values), values);

            foreach (var item in values)
            {
                self.Add(item.Key, item.Value);
            }

            return self;
        }

        /// <summary>
        /// Create read only dictionary by creating new dictionary with current keys and values.
        /// This is a shallow copy
        /// </summary>
        /// <typeparam name="TKey">key type</typeparam>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="comparer">equality comparer, optional</param>
        /// <returns>new read only dictionary</returns>
        public static IReadOnlyDictionary<TKey, TValue> ToImmutable<TKey, TValue>(this IDictionary<TKey, TValue> self, IEqualityComparer<TKey> comparer = null)
        {
            Verify.IsNotNull(nameof(self), self);

            if (comparer != null)
            {
                return self.ToDictionary(x => x.Key, x => x.Value, comparer);
            }
            else
            {
                return self.ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}
