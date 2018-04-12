// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Khooversoft.Toolbox
//{
//    public class ImmutableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
//    {
//        private readonly Dictionary<TKey, TValue> _dict;

//        private ImmutableDictionary()
//        {
//            _dict = new Dictionary<TKey, TValue>();
//        }

//        public ImmutableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items)
//        {
//            Verify.IsNotNull(nameof(items), items);

//            _dict = items.ToDictionary(x => x.Key, x => x.Value);
//        }

//        public ImmutableDictionary(IEqualityComparer<TKey> comparer)
//        {
//            Verify.IsNotNull(nameof(comparer), comparer);

//            _dict = new Dictionary<TKey, TValue>(comparer);
//        }

//        public ImmutableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items, IEqualityComparer<TKey> comparer)
//        {
//            Verify.IsNotNull(nameof(items), items);
//            Verify.IsNotNull(nameof(comparer), comparer);

//            _dict = items.ToDictionary(x => x.Key, x => x.Value, comparer);
//        }

//        public static ImmutableDictionary<TKey, TValue> Empty { get; } = new ImmutableDictionary<TKey, TValue>();

//        public TValue this[TKey key] => _dict[key];

//        public IEnumerable<TKey> Keys => _dict.Keys;

//        public IEnumerable<TValue> Values => _dict.Values;

//        public int Count => _dict.Count;

//        public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

//        public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);

//        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();

//        IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();
//    }
//}
