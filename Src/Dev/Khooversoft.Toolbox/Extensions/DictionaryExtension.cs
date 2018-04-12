// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Dictionary extensions for adding and removing values
    /// 
    /// There are two types of extensions, dictionary that have key is a string, object, or custom type
    /// 
    /// Using a name of type reduces the bugs arising with having keys being magic strings.  The
    /// type will generate the same key, every type.
    /// </summary>
    public static class DictionaryExtension
    {
        // ========================================================================================
        // String keys
        // ========================================================================================

        /// <summary>
        /// Get value from dictionary, use type name as key
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="throwNotFound">true, throw if key is not found</param>
        /// <returns>value, or null</returns>
        public static T Get<T>(this IDictionary<string, object> self, bool throwNotFound = false)
            where T : class
        {
            return self.Get<T>(typeof(T).Name);
        }

        /// <summary>
        /// Get value based on key
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="name">key</param>
        /// <param name="throwNotFound">true, throw if key is not found</param>
        /// <returns>value, or null</returns>
        public static T Get<T>(this IDictionary<string, object> self, string name, bool throwNotFound = false)
            where T : class
        {
            Verify.IsNotNull(nameof(self), self);

            object value;
            name = name ?? typeof(T).Name;
            if (!self.TryGetValue(name, out value))
            {
                if (throwNotFound)
                {
                    throw new ArgumentException(name, "Property in dictionary not found");
                }
                return null;
            }

            return (T)value;
        }

        /// <summary>
        /// Get value using type's name as key
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="throwNotFound">true, throw if key is not found</param>
        /// <returns>value or null if does not exist</returns>
        public static T Get<T>(this IReadOnlyDictionary<string, object> self, bool throwNotFound = false)
            where T : class
        {
            return self.Get<T>(typeof(T).Name);
        }

        /// <summary>
        /// Get value by name as key
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="name">key</param>
        /// <param name="throwNotFound">true, throw if key does not exist</param>
        /// <returns>value or null</returns>
        public static T Get<T>(this IReadOnlyDictionary<string, object> self, string name, bool throwNotFound = false)
            where T : class
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotEmpty(nameof(name), name);

            object value;
            if (!self.TryGetValue(name, out value))
            {
                if (throwNotFound)
                {
                    throw new ArgumentException(name, "Property in dictionary not found");
                }
                return null;
            }

            return (T)value;
        }

        /// <summary>
        /// Get or create value in dictionary using type's name as key or name specified
        /// </summary>
        /// <typeparam name="TValue">value retrieved or created</typeparam>
        /// <typeparam name="TCreate">type to create</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="create">delegate used to create</param>
        /// <param name="name">name (optional, will use type's name if not specified)</param>
        /// <returns>value retrieved or created</returns>
        public static TValue GetOrCreate<TValue, TCreate>(this IDictionary<string, object> self, Func<TCreate> create, string name = null)
            where TValue : class
            where TCreate : class, TValue
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(create), create);

            object value;
            name = name ?? typeof(TValue).Name;
            if (!self.TryGetValue(name, out value))
            {
                var result = create();
                self.Set(result);
                return result;
            }

            return (TValue)value;
        }

        /// <summary>
        /// Set value using type's name as key or name
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="value">value</param>
        /// <param name="name">name (optional, use type's name if not specified)</param>
        /// <returns>self</returns>
        public static IDictionary<string, object> Set<T>(this IDictionary<string, object> self, T value, string name = null)
            where T : class
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(value), value);

            name = name ?? typeof(T).Name;
            self[name] = value;

            return self;
        }

        /// <summary>
        /// Add if key does not exist
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="create">delegate to create</param>
        /// <param name="name">name (optional, use type's name if not specified)</param>
        /// <returns>selfs</returns>
        public static IDictionary<string, object> TryAdd<T>(this IDictionary<string, object> self, Func<T> create, string name = null)
            where T : class
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(create), create);

            name = name ?? typeof(T).Name;
            if (!self.ContainsKey(name))
            {
                self[name] = create();
            }

            return self;
        }

        /// <summary>
        /// Set value using derived type as key's name
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="value">value</param>
        /// <param name="derivedType">derived type to use as key</param>
        /// <returns>self</returns>
        public static IDictionary<string, object> Set<T>(this IDictionary<string, object> self, T value, Type derivedType)
            where T : class
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(value), value);
            Verify.IsNotNull(nameof(derivedType), derivedType);

            self[derivedType.Name] = value;

            return self;
        }

        /// <summary>
        /// Set value using type's name as key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDerivedType"></typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="value">value</param>
        /// <returns>self</returns>
        public static IDictionary<string, object> Set<T, TDerivedType>(this IDictionary<string, object> self, T value)
            where T : class
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(value), value);

            self[typeof(T).Name] = value;

            return self;
        }

        /// <summary>
        /// Remove from dictionary using value's type name as key
        /// </summary>
        /// <typeparam name="TValue">value's type</typeparam>
        /// <param name="self">dictionary</param>
        /// <returns>self</returns>
        public static IDictionary<string, object> Remove<TValue>(this IDictionary<string, object> self)
            where TValue : class
        {
            Verify.IsNotNull(nameof(self), self);

            self.Remove(typeof(TValue).Name);
            return self;
        }

        // ========================================================================================
        // Object keys
        // ========================================================================================

        /// <summary>
        /// Get value from dictionary
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="name">name (optional, use type's name if not specified)</param>
        /// <param name="throwNotFound">true, throw if key does not exist</param>
        /// <returns></returns>
        public static T Get<T>(this IDictionary<object, object> self, string name = null, bool throwNotFound = false)
            where T : class
        {
            Verify.IsNotNull(nameof(self), self);

            object value;
            name = name ?? typeof(T).Name;
            if (!self.TryGetValue(name, out value))
            {
                if (throwNotFound)
                {
                    throw new ArgumentException(name, "Property in dictionary not found");
                }
                return null;
            }

            return (T)value;
        }

        /// <summary>
        /// Get or create value if does not exit
        /// </summary>
        /// <typeparam name="TValue">value's type</typeparam>
        /// <typeparam name="TCreate">create type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="create">delegate to create value</param>
        /// <param name="name">name (optional, use's type name as key if not specified)</param>
        /// <returns></returns>
        public static TValue GetOrCreate<TValue, TCreate>(this IDictionary<object, object> self, Func<TCreate> create, string name = null)
            where TValue : class
            where TCreate : class, TValue
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(create), create);

            object value;
            name = name ?? typeof(TValue).Name;
            if (!self.TryGetValue(name, out value))
            {
                var result = create();
                self.Set(result);
                return result;
            }

            return (TValue)value;
        }

        /// <summary>
        /// Set value
        /// </summary>
        /// <typeparam name="T">value's type</typeparam>
        /// <param name="self">dictionary</param>
        /// <param name="value">value</param>
        /// <param name="name">name (optional, use key's name as key if not specified)</param>
        /// <returns>self</returns>
        public static IDictionary<object, object> Set<T>(this IDictionary<object, object> self, T value, string name = null)
            where T : class
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotNull(nameof(value), value);

            name = name ?? typeof(T).Name;
            self[name] = value;

            return self;
        }

        /// <summary>
        /// Remove based on type's name as key
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">dictionary</param>
        /// <returns>self</returns>
        public static IDictionary<object, object> Remove<T>(this IDictionary<object, object> self)
            where T : class
        {
            Verify.IsNotNull(nameof(self), self);

            self.Remove(typeof(T).Name);
            return self;
        }

        // ========================================================================================
        // Generic to all types
        // ========================================================================================

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

        // ========================================================================================
        // To Property Bag (Immutable)
        // ========================================================================================

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
