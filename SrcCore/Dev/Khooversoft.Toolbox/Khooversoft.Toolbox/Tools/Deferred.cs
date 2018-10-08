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
    /// Fast deferred execution using lambda
    /// </summary>
    /// <typeparam name="T">return type</typeparam>
    public class Deferred<T>
    {
        private T _value;
        private Func<T> _getValue;
        private object _lock = new object();

        /// <summary>
        /// Construct with value already established
        /// </summary>
        /// <param name="value">value to be returned</param>
        public Deferred(T value)
        {
            _value = value;
            _getValue = () => _value;
        }

        /// <summary>
        /// Construct with lambda to return value
        /// </summary>
        /// <param name="getValue"></param>
        public Deferred(Func<T> getValue)
        {
            Verify.IsNotNull(nameof(getValue), getValue);

            Func<T> f = getValue;
            _getValue = () => GetValue(f);
        }

        /// <summary>
        /// Return value (lazy)
        /// </summary>
        public T Value { get { return _getValue(); } }

        /// <summary>
        /// Get value by switching lambda, will only be called once
        /// </summary>
        /// <param name="getValue">lambda to get value</param>
        /// <returns>value</returns>
        private T GetValue(Func<T> getValue)
        {
            try
            {
                // Serialize access to the lambda for getting / creating object
                lock (_lock)
                {
                    _value = getValue();
                    _getValue = () => _value;
                    return _value;
                }
            }
            catch (Exception ex)
            {
                Exception exSave = ex;
                _getValue = () => throw exSave;
                throw ex;
            }
        }
    }
}
