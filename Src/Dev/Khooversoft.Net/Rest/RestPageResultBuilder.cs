// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;

namespace Khooversoft.Net
{
    /// <summary>
    /// Build REST page response
    /// </summary>
    /// <typeparam name="T">type of items</typeparam>
    public class RestPageResultBuilder<T>
    {
        public RestPageResultBuilder()
        {
        }

        /// <summary>
        /// Next URI (optional)
        /// </summary>
        public Uri NextUri { get; set; }

        /// <summary>
        /// Items in collection
        /// </summary>
        public IList<T> Items { get; } = new List<T>();

        /// <summary>
        /// Build Next URI based on response URI, index, and limit
        /// </summary>
        /// <param name="baseUri">based URI</param>
        /// <param name="index">starting index</param>
        /// <param name="limit">maximum limit to the number of items to return</param>
        /// <returns>this</returns>
        public RestPageResultBuilder<T> SetNextUri(Uri baseUri, string index, int limit)
        {
            Verify.IsNotNull(nameof(baseUri), baseUri);
            Verify.IsNotEmpty(nameof(index), index);
            Verify.Assert(limit > 0, nameof(limit));

            NextUri = new RestUriBuilder()
                .SetBaseUri(baseUri)
                .AddQuery("index", index)
                .AddQuery("limit", limit.ToString())
                .Build();

            return this;
        }

        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="value">value to add</param>
        /// <returns>this</returns>
        public RestPageResultBuilder<T> AddItem(T value)
        {
            Verify.IsNotDefault(nameof(value), value);

            Items.Add(value);
            return this;
        }

        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="values">value to add</param>
        /// <returns>this</returns>
        public RestPageResultBuilder<T> AddItem(IEnumerable<T> values)
        {
            Verify.IsNotNull(nameof(values), values);

            foreach (var item in values)
            {
                AddItem(item);
            }

            return this;
        }

        /// <summary>
        /// Build collection contract response
        /// </summary>
        /// <returns>response collection object</returns>
        public RestPageResultV1<T> Build()
        {
            Verify.Assert(Items.Count > 0, nameof(Items));

            return new RestPageResultV1<T>
            {
                ContinueIndexUri = NextUri?.ToString(),
                Items = new List<T>(Items)
            };
        }
    }
}
