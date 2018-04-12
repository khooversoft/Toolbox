// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Immutable list.  Shallow copies
    /// </summary>
    /// <typeparam name="T">type in list</typeparam>
    public class ImmutableList<T> : IReadOnlyList<T>
    {
        private readonly List<T> _list;

        /// <summary>
        /// Constructs a empty collection
        /// </summary>
        private ImmutableList()
        {
            _list = new List<T>();
        }

        /// <summary>
        /// Construct immutable collection from enumeration
        /// </summary>
        /// <param name="list"></param>
        public ImmutableList(IEnumerable<T> list)
        {
            Verify.IsNotNull(nameof(list), list);

            _list = new List<T>(list);
        }

        /// <summary>
        /// Empty list
        /// </summary>
        public static ImmutableList<T> Empty { get; } = new ImmutableList<T>();

        /// <summary>
        /// Get element at index
        /// </summary>
        /// <param name="index">index of element</param>
        /// <returns>element of T</returns>
        public T this[int index] => _list[index];

        /// <summary>
        /// Try to get value, out of range will return false
        /// </summary>
        /// <param name="index">index of element</param>
        /// <param name="value">value if index is within range</param>
        /// <returns>true if in range, false if not</returns>
        public bool TryGet(int index, out T value)
        {
            value = default(T);

            if (index <= 0 || index > _list.Count)
            {
                return false;
            }

            value = _list[index];
            return true;
        }

        /// <summary
        /// Count of elements
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Add value and create new immutable list. Current list is not modified.
        /// </summary>
        /// <param name="value">value to add</param>
        /// <returns>new immutable list</returns>
        public ImmutableList<T> Add(T value) => new ImmutableList<T>(_list.Concat(new T[] { value }));

        public ImmutableList<T> Remove(T value)
        {
            var newList = new ImmutableList<T>(_list);
            newList._list.Remove(value);
            return newList;
        }

        public IEnumerator<T> GetEnumerator() => _list.ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _list.ToList().GetEnumerator();
    }
}
