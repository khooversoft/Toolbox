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
    /// Immutable string Vector for parsing and construction of string using a delimiter for field separator
    /// Used for resource keys or paths
    ///
    /// If the vectors represent resource and key, the vectors would be "{source}/{key}"
    /// 
    /// String vector is a string using a delimiter to separate the vectors
    /// 
    /// E.g. "name/first/3" where "/" is the delimiter.
    /// </summary>
    public class StringVector : IEnumerable<string>
    {
        private readonly List<string> _values = new List<string>();
        private string _vectorValue;
        private Func<string> _getVectorValue;
        private const string _exceptionMsg = "Null or empty string are not allowed";

        public StringVector()
        {
            _getVectorValue = Build;
        }

        /// <summary>
        /// Create vector based on value and delimiter (default is "/")
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delimiter"></param>
        public StringVector(string value, string delimiter = null)
            : this()
        {
            Verify.IsNotEmpty(nameof(value), value);

            Delimiter = delimiter ?? Delimiter;
            _values = new List<string>(value.Split(new string[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries));
        }

        public StringVector(IEnumerable<string> values, string delimiter = null)
            : this()
        {
            Verify.IsNotNull(nameof(values), values);

            Delimiter = delimiter ?? Delimiter;
            _values = new List<string>(values);
            Verify.Assert(_values.All(x => x.IsNotEmpty()), _exceptionMsg);
        }

        private StringVector(IList<string> list, string delimiter = null)
            : this()
        {
            Verify.IsNotNull(nameof(list), list);

            Delimiter = delimiter ?? Delimiter;
            _values = new List<string>(list);
            Verify.Assert(_values.All(x => x.IsNotEmpty()), _exceptionMsg);
        }

        /// <summary>
        /// Empty string vector
        /// </summary>
        public static StringVector Empty { get; } = new StringVector();

        /// <summary>
        /// Delimiter for vectors
        /// </summary>
        public string Delimiter { get; private set; } = "/";

        /// <summary>
        /// Return value at index.  If the index is greater then the list, null is returned
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>null if out of range, value at index</returns>
        public string this[int index]
        {
            get
            {
                if (index >= _values.Count)
                {
                    return null;
                }

                return _values[index];
            }
        }

        /// <summary>
        /// Get count of vectors
        /// </summary>
        public int Count { get { return _values.Count; } }

        /// <summary>
        /// Get string representation of the vectors
        /// </summary>
        public string Value { get { return _getVectorValue(); } }

        /// <summary>
        /// Get vector value and convert it to type specified
        /// </summary>
        /// <typeparam name="T">type to convert to</typeparam>
        /// <param name="index">index of vector</param>
        /// <returns></returns>
        public T Get<T>(int index)
        {
            if (index >= _values.Count)
            {
                return default(T);
            }

            Type toType = typeof(T);
            if (toType.IsEnum)
            {
                return (T)Enum.Parse(toType, this[index], ignoreCase: true);
            }

            return (T)Convert.ChangeType(this[index], typeof(T));
        }

        /// <summary>
        /// Create new string vector with new delimiter
        /// </summary>
        /// <param name="delimiter">delimiter</param>
        /// <returns>new string vector</returns>
        public StringVector WithDelimiter(string delimiter)
        {
            Verify.IsNotEmpty(nameof(delimiter), delimiter);

            if (_values.Count == 0)
            {
                return new StringVector { Delimiter = delimiter };
            }

            return new StringVector(_values, delimiter);
        }

        /// <summary>
        /// Create new string vector with added vector value
        /// </summary>
        /// <param name="value">vector value to append</param>
        /// <returns>new string vector</returns>
        public StringVector With(string value)
        {
            Verify.IsNotEmpty(nameof(value), value);

            var sv = StringVector.Parse(value, Delimiter);
            return new StringVector(_values.Concat(sv), Delimiter);
        }

        /// <summary>
        /// Create new string vector with number of vectors value appended
        /// </summary>
        /// <param name="values">value to append</param>
        /// <returns>new string vector</returns>
        public StringVector With(IEnumerable<string> values)
        {
            Verify.IsNotNull(nameof(values), values);

            return new StringVector(_values.Concat(values), Delimiter);
        }

        /// <summary>
        /// With vector, concatenate vectors
        /// </summary>
        /// <param name="vector">vector</param>
        /// <returns>new vector</returns>
        public StringVector With(StringVector vector)
        {
            Verify.IsNotNull(nameof(vector), vector);

            return new StringVector(_values.Concat(vector));
        }

        /// <summary>
        /// Create new string vector with vector at index removed
        /// </summary>
        /// <param name="index">index of vector</param>
        /// <returns>new string vector</returns>
        public StringVector WithRemoveAt(int index)
        {
            Verify.Assert<ArgumentOutOfRangeException>(index >= 0, nameof(index));

            var list = new List<string>(this);
            list.RemoveAt(index);

            return new StringVector(list);
        }

        /// <summary>
        /// Create new string vector all instances of vector removed
        /// </summary>
        /// <param name="values">value to search for</param>
        /// <param name="ignoreCase">ignore case</param>
        /// <returns>new string vector with value if detected removed</returns>
        public StringVector WithRemove(string value, bool ignoreCase = false)
        {
            Verify.IsNotEmpty(nameof(value), value);

            return new StringVector(_values.Where(x => ignoreCase ? x.Equals(value, StringComparison.OrdinalIgnoreCase) : x != value));
        }

        /// <summary>
        /// Return string representation of vector
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Build string representation of vector
        /// </summary>
        /// <returns>string</returns>
        private string Build()
        {
            _vectorValue = string.Join(Delimiter, _values);
            _getVectorValue = () => _vectorValue;

            return _vectorValue;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)_values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<string>)_values).GetEnumerator();
        }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator string(StringVector source)
        {
            return source.Value;
        }

        /// <summary>
        /// Parse string vector
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="delimiter">(optional) delimiter</param>
        /// <returns>string vector</returns>
        public static StringVector Parse(string value, string delimiter = null)
        {
            return new StringVector(value, delimiter);
        }
    }
}
