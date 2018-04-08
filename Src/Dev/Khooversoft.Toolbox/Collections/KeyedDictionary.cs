using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Modification of standard dictionary that uses delegate to extract or calculate the key.
    /// Optional delegate's for before add and after remove.  This allows additional linking
    /// or tracking.
    /// </summary>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyedDictionary<TKey, TValue> : IEnumerable<TValue>
    {
        private readonly Dictionary<TKey, TValue> _store;
        private readonly Func<TValue, TValue> _onBeforeAdd;
        private readonly Action<TValue> _onAfterRemove;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getKey">get key delegate</param>
        public KeyedDictionary(Func<TValue, TKey> getKey)
        {
            Verify.IsNotNull(nameof(getKey), getKey);

            _store = new Dictionary<TKey, TValue>();
            GetKey = getKey;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getKey">get key delegate</param>
        /// <param name="comparer">key comparer</param>
        public KeyedDictionary(Func<TValue, TKey> getKey, IEqualityComparer<TKey> comparer)
            : this(getKey)
        {
            Verify.IsNotNull(nameof(comparer), comparer);

            _store = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getKey">get key delegate</param>
        /// <param name="items">Load items</param>
        public KeyedDictionary(Func<TValue, TKey> getKey, IEnumerable<TValue> items)
            : this(getKey)
        {
            Verify.IsNotNull(nameof(items), items);

            _store = new Dictionary<TKey, TValue>();
            Set(items);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getKey">get key delegate</param>
        /// <param name="items">Load items</param>
        /// <param name="comparer">key comparer</param>
        public KeyedDictionary(Func<TValue, TKey> getKey, IEnumerable<TValue> items, IEqualityComparer<TKey> comparer)
            : this(getKey)
        {
            _store = items.ToDictionary(x => GetKey(x), x => x, comparer);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getKey">get key delegate</param>
        /// <param name="onBeforeAdd">delegate for before add, return value is added to dictionary</param>
        /// <param name="onAfterRemove">delegate for after remove</param>
        public KeyedDictionary(Func<TValue, TKey> getKey, Func<TValue, TValue> onBeforeAdd = null, Action<TValue> onAfterRemove = null)
            : this(getKey)
        {
            _onBeforeAdd = onBeforeAdd;
            _onAfterRemove = onAfterRemove;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getKey">get key delegate</param>
        /// <param name="items">Load items</param>
        /// <param name="onBeforeAdd">delegate for before add, return value is added to dictionary</param>
        /// <param name="onAfterRemove">delegate for after remove</param>
        public KeyedDictionary(Func<TValue, TKey> getKey, IEnumerable<TValue> items, Func<TValue, TValue> onBeforeAdd = null, Action<TValue> onAfterRemove = null)
            : this(getKey, items)
        {
            _onBeforeAdd = onBeforeAdd;
            _onAfterRemove = onAfterRemove;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="getKey">get key delegate</param>
        /// <param name="items">Load items</param>
        /// <param name="comparer">key comparer</param>
        /// <param name="onBeforeAdd">delegate for before add, return value is added to dictionary</param>
        /// <param name="onAfterRemove">delegate for after remove</param>
        public KeyedDictionary(Func<TValue, TKey> getKey, IEnumerable<TValue> items, IEqualityComparer<TKey> comparer, Func<TValue, TValue> onBeforeAdd = null, Action<TValue> onAfterRemove = null)
            : this(getKey, items, comparer)
        {
            _onBeforeAdd = onBeforeAdd;
            _onAfterRemove = onAfterRemove;
        }

        /// <summary>
        /// Get key delegate
        /// </summary>
        public Func<TValue, TKey> GetKey { get; }

        /// <summary>
        /// Gets the value associated with the specified key 
        /// </summary>
        /// <param name="key">The key of the value to get</param>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The key does not exist in the collection</exception>
        /// <returns></returns>
        public TValue this[TKey key] { get => _store[key]; }

        /// <summary>
        /// The number of values in the collection
        /// </summary>
        public int Count => _store.Count;

        /// <summary>
        /// List keys
        /// </summary>
        public IEnumerable<TKey> Keys => _store.Keys;

        /// <summary>
        /// List values
        /// </summary>
        public IEnumerable<TValue> Values => _store.Values;

        /// <summary>
        /// Clear the collection
        /// </summary>
        /// <returns>this</returns>
        public KeyedDictionary<TKey, TValue> Clear()
        {
            _store.Clear();
            return this;
        }

        /// <summary>
        /// Add value, GetKey delegate is used to retrieve the key.
        /// </summary>
        /// <param name="value">value</param>
        /// <exception cref="System.ArgumentException">Key already exists</exception>
        /// <returns>this</returns>
        public KeyedDictionary<TKey, TValue> Add(TValue value)
        {
            Verify.IsNotNull(nameof(value), value);

            _onBeforeAdd?.Invoke(value);
            _store.Add(GetKey(value), value);
            return this;
        }

        /// <summary>
        /// Update or Add value, GetKey delegate is used to retrieve the key.
        /// If key does not exist, it is created with value
        /// If key does exist, the value is updated
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>this</returns>
        public KeyedDictionary<TKey, TValue> Set(TValue value)
        {
            Verify.IsNotNull(nameof(value), value);

            _onBeforeAdd?.Invoke(value);
            _store[GetKey(value)] = value;
            return this;
        }

        /// <summary>
        /// Update or Add values, GetKey delegate is used to retrieve the key.
        /// </summary>
        /// <param name="items">values</param>
        /// <returns>this</returns>
        public KeyedDictionary<TKey, TValue> Set(IEnumerable<TValue> items)
        {
            Verify.IsNotNull(nameof(items), items);

            foreach (var item in items)
            {
                _onBeforeAdd?.Invoke(item);
                _store[GetKey(item)] = item;
            }

            return this;
        }

        /// <summary>
        /// Remove value associated with key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true if removed, false if not</returns>
        public bool Remove(TKey key)
        {
            Verify.IsNotNull(nameof(key), key);

            TValue value;
            if (_store.TryGetValue(key, out value))
            {
                _store.Remove(key);
                _onAfterRemove?.Invoke(value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Test if collection contains key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true if key is in the collection, false if not</returns>
        public bool ContainsKey(TKey key)
        {
            Verify.IsNotNull(nameof(key), key);

            return _store.ContainsKey(key);
        }

        /// <summary>
        /// Try to get value associated with key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value to return</param>
        /// <returns>true if value returned, false if not</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            Verify.IsNotNull(nameof(key), key);

            return _store.TryGetValue(key, out value);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _store.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Operator to set value to collection
        /// </summary>
        /// <param name="self">class reference</param>
        /// <param name="value">value to set</param>
        /// <returns>self</returns>
        public static KeyedDictionary<TKey, TValue> operator +(KeyedDictionary<TKey, TValue> self, TValue value)
        {
            self.Set(value);
            return self;
        }

        /// <summary>
        /// Operator to set values to collection
        /// </summary>
        /// <param name="self">class reference</param>
        /// <param name="values">values to set</param>
        /// <returns>self</returns>
        public static KeyedDictionary<TKey, TValue> operator +(KeyedDictionary<TKey, TValue> self, IEnumerable<TValue> values)
        {
            self.Set(values);
            return self;
        }
    }
}
