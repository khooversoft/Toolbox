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
    /// Utility class for binding a vector value(s) to a class property / variable
    /// </summary>
    public class StringVectorBind
    {
        private readonly List<Bind> _bindings = new List<Bind>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requiredVectors">if specified, number of required vectors, null = all required</param>
        /// <param name="maxVectors">if specified, maximum number of vectors</param>
        public StringVectorBind(int? requiredVectors = null, int? maxVectors = null)
        {
            Verify.Assert(requiredVectors == null || maxVectors == null || (requiredVectors <= maxVectors), "required vs max vectors out of range");

            RequiredVectors = requiredVectors;
            MaxVectors = maxVectors;
        }

        /// <summary>
        /// Required number of vectors, if specified
        /// </summary>
        public int? RequiredVectors { get; set; }

        /// <summary>
        /// Max vectors, if specified
        /// </summary>
        public int? MaxVectors { get; set; }

        /// <summary>
        /// Add binding for a vector
        /// </summary>
        /// <param name="get">get function</param>
        /// <param name="set">set function</param>
        /// <returns>this</returns>
        public StringVectorBind Add(Func<string> get, Action<string> set)
        {
            _bindings.Add(new Bind(get, set));

            return this;
        }

        /// <summary>
        /// Insert in binding for vector
        /// </summary>
        /// <param name="index">insert into index</param>
        /// <param name="get">get function</param>
        /// <param name="set">set function</param>
        /// <returns>this</returns>
        public StringVectorBind Insert(int index, Func<string> get, Action<string> set)
        {
            _bindings.Insert(index, new Bind(get, set));

            return this;
        }

        /// <summary>
        /// Parse and set vector value(s)
        /// </summary>
        /// <param name="sv">string vector</param>
        public void Set(string sv)
        {
            Verify.IsNotEmpty(nameof(sv), sv);

            Set(new StringVector(sv));
        }

        /// <summary>
        /// Set vector value(s)
        /// </summary>
        /// <param name="sv">string vector</param>
        public void Set(StringVector sv)
        {
            Verify.IsNotNull(nameof(sv), sv);
            Verify.Assert(RequiredVectors == null || RequiredVectors <= sv.Count, "Vector count is less than required vectors");
            Verify.Assert(MaxVectors == null || sv.Count <= MaxVectors, "Max vectors has been exceeded");

            int max = Math.Min(_bindings.Count, sv.Count);
            for (int index = 0; index < max; index++)
            {
                _bindings[index].Set(sv[index]);
            }
        }

        /// <summary>
        /// Get string vector
        /// </summary>
        /// <param name="maxCount">if specified, max number of vectors</param>
        /// <returns>string vector</returns>
        public StringVector Get(int? maxCount = null)
        {
            List<string> values = new List<string>();

            foreach (var item in _bindings)
            {
                string v = item.Get();
                if (v.IsEmpty())
                {
                    break;
                }

                values.Add(v);

                if (maxCount != null && values.Count == maxCount)
                {
                    break;
                }
            }

            return new StringVector(values);
        }

        /// <summary>
        /// Private bind class
        /// </summary>
        private class Bind
        {
            public Bind(Func<string> get, Action<string> set)
            {
                Verify.IsNotNull(nameof(get), get);
                Verify.IsNotNull(nameof(set), set);

                Get = get;
                Set = set;
            }

            public Action<string> Set { get; }

            public Func<string> Get { get; }
        }
    }
}
