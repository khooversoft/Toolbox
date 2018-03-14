using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Extensions for the Service configuration
    /// 
    /// Service configuration comes in 2 forms, mutable and immutable (via interface)
    /// </summary>
    public static class ServiceConfigurationExtensions
    {
        /// <summary>
        /// Set value using type as key
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">service configuration</param>
        /// <param name="value">value to set (add or update)</param>
        /// <returns>self</returns>
        public static ServiceConfiguration Set<T>(this ServiceConfiguration self, T value) where T : class
        {
            Verify.IsNotNull(nameof(self), self);

            self.Properties.Set(value);
            return self;
        }

        /// <summary>
        /// Set value (add or update)
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">service configuration</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>self</returns>
        public static ServiceConfiguration Set<T>(this ServiceConfiguration self, string key, T value) where T : class
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotEmpty(nameof(key), key);

            self.Properties.Set(value, key);
            return self;
        }

        /// <summary>
        /// Get value based on type name as key
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">service configuration</param>
        /// <param name="throwNotFound">true, throw if key is not found</param>
        /// <returns>value, null if not found</returns>
        public static T Get<T>(this IServiceConfiguration self, bool throwNotFound = false) where T : class
        {
            Verify.IsNotNull(nameof(self), self);

            return self.Properties.Get<T>(throwNotFound: throwNotFound);
        }

        /// <summary>
        /// Get value based on key
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="self">Service configuration</param>
        /// <param name="key">key</param>
        /// <param name="throwNotFound">true, throw if key is not found</param>
        /// <returns>value, null if not found</returns>
        public static T Get<T>(this IServiceConfiguration self, string key, bool throwNotFound = false) where T : class
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotEmpty(nameof(key), key);

            return self.Properties.Get<T>(key, throwNotFound: throwNotFound);
        }

        /// <summary>
        /// Get setting (properties)
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        public static string GetSetting(this IServiceConfiguration self, string key)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotEmpty(nameof(key), key);

            object value;
            if (self.Properties.TryGetValue(key, out value))
            {
                return value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Set settings (set)
        /// </summary>
        /// <param name="key">key (required)</param>
        /// <param name="value">value, can be null</param>
        /// <returns>self</returns>
        public static ServiceConfiguration SetSetting(this ServiceConfiguration self, string key, string value)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotEmpty(nameof(key), key);

            self.Properties.Set(value, key);
            return self;
        }

    }
}
