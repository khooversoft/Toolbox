using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// LRU (Least Recently Used) list.  The least used item are in the beginning of the list, while the most used
    /// are at the end of the list.
    /// 
    /// Enumerator for LRU Cache item (details accessed count and last date)
    /// </summary>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="T">type to store</typeparam>
    public class LruCache<TKey, T> : IEnumerable<LruCache<TKey, T>.CacheItem>
    {
        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cacheMap;
        private LinkedList<CacheItem> _lruList = new LinkedList<CacheItem>();
        private readonly object _lock = new object();

        public LruCache(int capacity)
        {
            Verify.Assert(capacity > 0, $"{nameof(capacity)} must be greater than 0");
            Capacity = capacity;

            _cacheMap = new Dictionary<TKey, LinkedListNode<CacheItem>>();
        }

        public LruCache(int capacity, IEqualityComparer<TKey> comparer)
            : this(capacity)
        {
            _cacheMap = new Dictionary<TKey, LinkedListNode<CacheItem>>(comparer);
        }

        /// <summary>
        /// Event when cache item has been removed
        /// </summary>
        public event Action<CacheItem> CacheItemRemoved;

        /// <summary>
        /// Get or set value based on key.  If key does not exist, return default(T)
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>default(T) or value</returns>
        public T this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out T value))
                {
                    return value;
                }

                return default(T);
            }
            set
            {
                Set(key, value);
            }
        }

        /// <summary>
        /// Capacity of LRU cache (cannot be changed)
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Current count of cache
        /// </summary>
        public int Count { get { return _lruList.Count; } }

        /// <summary>
        /// Return values in LRU cache (least used to most used)
        /// </summary>
        public IEnumerable<T> Values
        {
            get
            {
                List<T> list;

                lock (_lock)
                {
                    list = _lruList.Select(x => x.Value).ToList();
                }

                return list;
            }
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <returns>this</returns>
        public LruCache<TKey, T> Clear()
        {
            lock (_lock)
            {
                _cacheMap.Clear();
                _lruList.Clear();
            }

            return this;
        }

        /// <summary>
        /// Set cache item.  If value already exist, remove it and add the new one
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>this</returns>
        public LruCache<TKey, T> Set(TKey key, T value)
        {
            Verify.IsNotNull(nameof(key), key);

            lock (_lock)
            {
                if (_cacheMap.TryGetValue(key, out LinkedListNode<CacheItem> node))
                {
                    _lruList.Remove(node);
                }
                else
                {
                    if (_cacheMap.Count >= Capacity)
                    {
                        RemoveFirst();
                    }

                    var cacheItem = new CacheItem(key, value);
                    node = new LinkedListNode<CacheItem>(cacheItem);
                }

                _lruList.AddLast(node);
                _cacheMap[key] = node;
            }

            return this;
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true if removed, false if not</returns>
        public bool Remove(TKey key)
        {
            return TryRemove(key, out T value);
        }

        /// <summary>
        /// Try to get value from cache
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value to load if found</param>
        /// <param name="markUsed">true to mark cache item used, false will not</param>
        /// <returns>true if found, false if not</returns>
        public bool TryGetValue(TKey key, out T value, bool markUsed = true)
        {
            Verify.IsNotNull(nameof(key), key);

            value = default(T);
            lock (_lock)
            {
                LinkedListNode<CacheItem> node;
                if (_cacheMap.TryGetValue(key, out node))
                {
                    value = node.Value.Value;

                    if (markUsed)
                    {
                        node.Value.RecordAccessed();
                        _lruList.Remove(node);
                        _lruList.AddLast(node);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Try to remove key, set out value
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value to set</param>
        /// <returns>true if removed, false if not</returns>
        public bool TryRemove(TKey key, out T value)
        {
            Verify.IsNotNull(nameof(key), key);

            lock (_lock)
            {
                if (!_cacheMap.TryGetValue(key, out LinkedListNode<CacheItem> node))
                {
                    value = default(T);
                    return false;
                }

                _lruList.Remove(node);
                _cacheMap.Remove(key);

                value = node.Value.Value;
                return true;
            }
        }

        /// <summary>
        /// Get cache item details
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>cache item details or null if not found</returns>
        public CacheItem GetCacheDetails(TKey key)
        {
            Verify.IsNotNull(nameof(key), key);

            lock (_lock)
            {
                LinkedListNode<CacheItem> node;
                if (_cacheMap.TryGetValue(key, out node))
                {
                    return node.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Return enumerator of cache item details, from lease used to most used
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator<CacheItem> GetEnumerator()
        {
            List<CacheItem> list;

            lock (_lock)
            {
                list = new List<CacheItem>(_lruList);
            }

            foreach (var item in list)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void RemoveFirst()
        {
            // Remove from LRUPriority
            LinkedListNode<CacheItem> node = _lruList.First;
            _lruList.RemoveFirst();

            // Remove from cache
            _cacheMap.Remove(node.Value.Key);

            // Notify of event removed
            CacheItemRemoved?.Invoke(node.Value);
        }

        [DebuggerDisplay("Key={Key}, AccessedCount={AccessedCount}, LastAccessed={LastAccessed}")]
        public class CacheItem
        {
            internal CacheItem(TKey key, T value)
            {
                Key = key;
                Value = value;
                LastAccessed = DateTimeOffset.UtcNow;
                AccessedCount = 1;
            }

            public TKey Key;

            public T Value;

            public DateTimeOffset LastAccessed { get; private set; }

            public int AccessedCount { get; private set; }

            public void RecordAccessed()
            {
                LastAccessed = DateTimeOffset.UtcNow;
                AccessedCount++;
            }
        }
    }
}
