// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Immutable execution context
    /// </summary>
    public sealed class WorkContext : IWorkContext
    {
        /// <summary>
        /// Default constructor for Empty
        /// </summary>
        private WorkContext()
        {
            Cv = new CorrelationVector();
            Tag = Tag.Empty;
            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Internal constructor for new instances
        /// </summary>
        /// <param name="workContext">copy from work context</param>
        private WorkContext(WorkContext workContext)
        {
            Cv = workContext.Cv;
            Tag = workContext.Tag;
            Container = workContext.Container;
            Properties = workContext.Properties.ToDictionary(x => x.Key, x => x.Value);
            CancellationToken = workContext.CancellationToken;
        }

        /// <summary>
        /// Construct work context, for values that are not known to be immutable, shallow copies are made
        /// </summary>
        /// <param name="cv">correlation vector</param>
        /// <param name="tag">code location tag</param>
        /// <param name="workContainer">container</param>
        /// <param name="properties">properties (optional)</param>
        public WorkContext(CorrelationVector cv, Tag tag, ILifetimeScope workContainer, IEnumerable<KeyValuePair<string, object>> properties = null, CancellationToken? cancellationToken = null)
        {
            Verify.IsNotNull(nameof(cv), cv);
            Verify.IsNotNull(nameof(tag), tag);

            Cv = cv;
            Tag = tag;
            Container = workContainer;
            Properties = properties.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, object>();
            CancellationToken = cancellationToken ?? CancellationToken.None;
        }

        /// <summary>
        /// Static empty
        /// </summary>
        public static WorkContext Empty { get; } = new WorkContext();

        public CorrelationVector Cv { get; private set; }

        public Tag Tag { get; private set; }

        public ILifetimeScope Container { get; }

        public IReadOnlyDictionary<string, object> Properties { get; }

        public CancellationToken CancellationToken { get; private set; } = CancellationToken.None;

        /// <summary>
        /// Create new instance of work context with key and value being added to properties
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>new work context</returns>
        public IWorkContext With(string key, object value)
        {
            Verify.IsNotEmpty(nameof(key), key);
            Verify.IsNotNull(nameof(value), value);

            return new WorkContextBuilder(this)
                .Add(key, value)
                .Build();
        }

        /// <summary>
        /// Create new instance of work context with new value being set in property (type name is used as the key)
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="value">value</param>
        /// <returns>new work context</returns>
        public IWorkContext With<T>(T value) where T : class
        {
            Verify.IsNotNull(nameof(value), value);

            return new WorkContextBuilder(this)
                .Set<T>(value)
                .Build();
        }

        /// <summary>
        /// Create new instance of work context with Increment CV
        /// </summary>
        /// <returns>new work context</returns>
        public IWorkContext WithExtended()
        {
            return new WorkContext(this)
            {
                Cv = Cv.WithExtend(),
            };
        }

        /// <summary>
        /// Create new instance of work context with Increment CV
        /// </summary>
        /// <returns>new work context</returns>
        public IWorkContext WithIncrement()
        {
            return new WorkContext(this)
            {
                Cv = Cv.WithIncrement(),
            };
        }

        /// <summary>
        /// Create new instance of work context with method name being added to Tag
        /// </summary>
        /// <param name="memberName">member name (compiler will fill in)</param>
        /// <returns>new work context</returns>
        public IWorkContext WithMethodName([CallerMemberName] string memberName = null)
        {
            return new WorkContext(this)
            {
                Tag = Tag.With(memberName),
            };
        }

        /// <summary>
        /// Create new instance of work context with Tag being added to current Tag
        /// </summary>
        /// <param name="tag">code tag</param>
        /// <param name="memberName">method name (compiler will fill in)</param>
        /// <returns>new work context</returns>
        public IWorkContext WithTag(Tag tag, [CallerMemberName] string memberName = null)
        {
            Verify.IsNotNull(nameof(tag), tag);

            return new WorkContext(this)
            {
                Tag = Tag.With(tag.WithMethodName(memberName)),
            };
        }

        /// <summary>
        /// Create new context with cancellation token
        /// </summary>
        /// <param name="token">token to set, null to clear</param>
        /// <returns>new work context</returns>
        public IWorkContext With(CancellationToken? token)
        {
            return new WorkContext(this)
            {
                CancellationToken = token ?? CancellationToken.None
            };
        }

        /// <summary>
        /// Create work context builder class from current work context
        /// </summary>
        /// <returns>builder class</returns>
        public WorkContextBuilder ToBuilder()
        {
            return new WorkContextBuilder(this);
        }
    }
}
