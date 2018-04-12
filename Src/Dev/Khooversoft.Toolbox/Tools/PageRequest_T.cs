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
    /// Page request based on a query
    /// </summary>
    /// <typeparam name="T">type of query</typeparam>
    public class PageRequest<T> : PageRequest
    {
        public PageRequest(T query, int limit, string index = null)
            : base(limit, index)
        {
            Query = query;
        }

        /// <summary>
        /// Query for page request
        /// </summary>
        public T Query { get; }
    }
}
