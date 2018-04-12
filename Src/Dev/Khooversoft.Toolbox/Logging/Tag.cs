// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Immutable tag that supports logging code location breadcrumb
    /// 
    /// Root is "/"
    /// </summary>
    [DebuggerDisplay("Value={Value}")]
    public sealed class Tag
    {
        private const string _forwardSlash = "/";

        public Tag()
        {
            this.Value = _forwardSlash;
        }

        public Tag(string tag)
        {
            Verify.IsNotNull(nameof(tag), tag);
            this.Value = tag;
        }

        public Tag(object obj, [CallerMemberName] string memberName = null)
        {
            Verify.IsNotNull(nameof(obj), obj);

            if (memberName == ".ctor")
            {
                this.Value = _forwardSlash + obj.GetType().Name;
            }
            else
            {
                this.Value = _forwardSlash + obj.GetType().Name + "." + memberName;
            }
        }

        public static Tag Empty => new Tag();

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// New immutable with value
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>new Tag</returns>
        public Tag With(string value)
        {
            Verify.IsNotEmpty(nameof(value), value);

            return new Tag(Value == _forwardSlash ? Value + value : Value + "." + value);
        }

        /// <summary>
        /// New immutable with tag
        /// </summary>
        /// <param name="tag">tag</param>
        /// <returns>new Tag</returns>
        public Tag With(Tag tag)
        {
            Verify.IsNotNull(nameof(tag), tag);

            int index = 0;
            if (tag.Value.Length > 0 && tag.Value[0] == _forwardSlash[0])
            {
                index = 1;
            }

            return new Tag(Value + "." + tag.Value.Substring(index));
        }

        /// <summary>
        /// New immutable with member name
        /// </summary>
        /// <param name="memberName">compiler method name</param>
        /// <returns>new Tag</returns>
        public Tag WithMethodName([CallerMemberName] string memberName = null)
        {
            return new Tag(this.Value + "." + memberName);
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value;
        }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator string(Tag source)
        {
            return source.Value;
        }
    }
}
