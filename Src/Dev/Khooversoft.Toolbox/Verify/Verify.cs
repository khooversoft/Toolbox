using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class Verify
    {
        /// <summary>
        /// Insures string is not empty (null or empty)
        /// </summary>
        /// <param name="name">name of variable</param>
        /// <param name="value">string to test</param>
        /// <param name="maxSize">(optional) max size of string</param>
        [DebuggerStepThrough]
        public static void IsNotEmpty(string name, string value, int? maxSize = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), "Null or empty");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(name), "Null or empty");
            }

            if (maxSize != null)
            {
                int testSize = (int)maxSize;
                if (testSize <= 0)
                {
                    throw new ArgumentException("Size must be greater then 0", nameof(maxSize));
                }

                if (value.Length > testSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(maxSize), $"String length {value.Length} is over max of {testSize}");
                }
            }
        }

        /// <summary>
        /// Test is value is not null
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="value">value</param>
        /// <param name="name">name</param>
        [DebuggerStepThrough]
        public static void IsNotNull<T>(string name, T value)
        {
            Verify.IsNotEmpty(nameof(name), name);

            if (value == null)
            {
                throw new ArgumentNullException(name, "Is NULL");
            }
        }

        /// <summary>
        /// Assert test and throw exception with message
        /// </summary>
        /// <typeparam name="T">type of exception</typeparam>
        /// <param name="test">test</param>
        /// <param name="message">exception message optional</param>
        [DebuggerStepThrough]
        public static void Assert<T>(bool test, string message = null) where T : Exception
        {
            Verify.IsNotEmpty(nameof(message), message);

            if (test == false)
            {
                if (message.IsEmpty())
                {
                    Exception exception = (Exception)Activator.CreateInstance(typeof(T));
                    throw exception;
                }
                else
                {
                    Exception exception = (Exception)Activator.CreateInstance(typeof(T), message);
                    throw exception;
                }
            }
        }

        /// <summary>
        /// Insure the value is not default
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="exp">member</param>
        /// <param name="value">value to test</param>
        [DebuggerStepThrough]
        public static void IsNotDefault<T>(string name, T value)
        {
            Verify.IsNotEmpty(name, nameof(name));

            if (object.Equals(value, default(T)))
            {
                throw new ArgumentException("Value must be default", name);
            }

            if (typeof(T) == typeof(string))
            {
                Verify.IsNotEmpty(name, (string)(object)value);
            }
        }

        /// <summary>
        /// Assert test
        /// </summary>
        /// <param name="state">state to test</param>
        /// <param name="message">message</param>
        [DebuggerStepThrough]
        public static void Assert(bool state, string message)
        {
            Verify.IsNotEmpty(nameof(message), message);

            Assert<ArgumentException>(state, message);
        }

        /// <summary>
        /// Assert that an exception was thrown
        /// </summary>
        /// <typeparam name="T">exception type</typeparam>
        /// <param name="action">action to execute</param>
        [DebuggerStepThrough]
        public static void AssertException<T>(Action action) where T : Exception
        {
            Verify.IsNotNull(nameof(action), action);

            try
            {
                action();
                throw new Exception($"Exception {typeof(T).Name} was not encountered");
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(T))
                {
                    throw new Exception($"Exception {typeof(T).Name} was not throw, exception detected is {ex.GetType().Name}");
                }
            }
        }

        /// <summary>
        /// Assert that an exception was thrown
        /// </summary>
        /// <typeparam name="T">exception type</typeparam>
        /// <param name="action">action to execute</param>
        [DebuggerStepThrough]
        public static async Task AssertExceptionAsync<T>(Func<Task> action) where T : Exception
        {
            Verify.IsNotNull(nameof(action), action);

            try
            {
                await action();
                throw new Exception($"Exception {typeof(T).Name} was not encountered");
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(T))
                {
                    throw new Exception($"Exception {typeof(T).Name} was not throw, exception detected is {ex.GetType().Name}");
                }
            }
        }

        /// <summary>
        /// Verify that user type is valid
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="name"></param>
        /// <param name="userType"></param>
        /// <param name="allowNull"></param>
        public static void IsValueValid<T>(string name, ICustomType<T> userType, bool allowNull = false)
        {
            Verify.IsNotEmpty(nameof(name), name);
            Verify.Assert<ArgumentNullException>(allowNull || userType != null, name);

            if (userType != null)
            {
                Verify.Assert(userType.IsValueValid(), name);
            }
        }
    }
}
