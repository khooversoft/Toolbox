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
    /// Page result from stores
    /// </summary>
    /// <typeparam name="T">type of elements</typeparam>
    public class PageResult<T>
    {
        public PageResult(IEnumerable<T> items)
        {
            Verify.IsNotNull(nameof(items), items);

            Items = new List<T>(items);
        }

        public PageResult(string index, IEnumerable<T> items)
            : this(items)
        {
            Index = index;
        }

        public PageResult(IEnumerable<T> items, int skip, int limit, int count)
            : this(items)
        {
            Verify.Assert(skip >= 0, nameof(skip));
            Verify.Assert(limit > 0, nameof(limit));
            Verify.Assert(count >= 0, nameof(count));

            Limit = limit;
            int nextIndex = skip + limit;
            Index = nextIndex < count ? nextIndex.ToString() : null;
        }

        public string Index { get; }

        public int Limit { get; }

        public IEnumerable<T> Items { get; }
    }
}
