using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Cache object, valid for only a specific amount of time specified in lifetime.
    /// If specified, refresh, can indicate when a refresh operations should be done
    /// </summary>
    /// <typeparam name="T">type of object cached</typeparam>
    public class CacheObject<T>
    {
        private DateTime? _validUntil;
        private DateTime? _refreshAt;
        private T _value;
        private readonly object _lock = new object();

        /// <summary>
        /// Construct a cache
        /// </summary>
        /// <param name="lifeTime">cache lifetime</param>
        /// <param name="refresh">refresh lifetime (optional)</param>
        public CacheObject(TimeSpan lifeTime, TimeSpan? refresh = null)
        {
            LifeTime = lifeTime;
            Refresh = refresh;
        }

        /// <summary>
        /// Cache lifetime setting
        /// </summary>
        public TimeSpan LifeTime { get; }

        /// <summary>
        /// If specified, refresh setting
        /// </summary>
        public TimeSpan? Refresh { get; }

        /// <summary>
        /// Clear cache, clear value and timeouts
        /// </summary>
        /// <returns>this</returns>
        public CacheObject<T> Clear()
        {
            lock (_lock)
            {
                _value = default(T);
                _validUntil = null;
                _refreshAt = null;
            }

            return this;
        }

        /// <summary>
        /// Try to get value
        /// </summary>
        /// <param name="value">return value</param>
        /// <returns>true if value is available, false if cache has expired</returns>
        public bool TryGetValue(out T value)
        {
            value = default(T);

            lock (_lock)
            {
                if (!IsValid())
                {
                    Clear();
                    return false;
                }

                value = _value;
                return true;
            }
        }

        /// <summary>
        /// Set value and reset clocks
        /// </summary>
        /// <param name="value">value to set</param>
        /// <returns>this</returns>
        public CacheObject<T> Set(T value)
        {
            lock (_lock)
            {
                _validUntil = DateTime.Now + LifeTime;
                _refreshAt = Refresh != null ? DateTime.Now + (TimeSpan)Refresh : (DateTime?)null;
                _value = value;
                return this;
            }
        }

        /// <summary>
        /// Is cache valid
        /// </summary>
        /// <returns>true if valid, false if expired</returns>
        public bool IsValid()
        {
            return _validUntil != null && DateTimeOffset.Now < _validUntil;
        }

        /// <summary>
        /// Has refresh clock expired
        /// </summary>
        /// <returns>true if refresh is required</returns>
        public bool IsRefresh()
        {
            if (_refreshAt == null)
            {
                return false;
            }

            return DateTime.Now >= (DateTime)_refreshAt;
        }
    }
}
