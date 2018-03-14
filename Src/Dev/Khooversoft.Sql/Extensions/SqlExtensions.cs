using Khooversoft.Toolbox;
using System;
using System.Data.SqlClient;

namespace Khooversoft.Sql
{
    public static class SqlExtensions
    {
        /// <summary>
        /// Get value from SQL Data Reader
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="reader">SQL data reader</param>
        /// <param name="name">name of field</param>
        /// <param name="optional">value is optional</param>
        /// <returns>type or default of optional</returns>
        public static T Get<T>(this SqlDataReader reader, string name, bool optional = false)
        {
            Verify.IsNotNull(nameof(reader), reader);
            Verify.IsNotEmpty(nameof(name), name);

            int ordinal;
            try
            {
                ordinal = reader.GetOrdinal(name);
            }
            catch (IndexOutOfRangeException)
            {
                if (optional)
                {
                    return default(T);
                }

                throw;
            }

            if (reader.IsDBNull(ordinal))
            {
                return default(T);
            }

            object value = reader.GetValue(ordinal);

            Type type = typeof(T);

            if (type == typeof(string) || Nullable.GetUnderlyingType(type) != null)
            {
                return (T)value;
            }

            if (typeof(T) == typeof(Guid))
            {
                return (T)(object)Guid.Parse(value.ToString());
            }

            return (T)Convert.ChangeType(value, type);
        }

        /// <summary>
        /// Parse enum from database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <param name="optional"></param>
        /// <returns></returns>
        public static T Parse<T>(this SqlDataReader reader, string name, bool optional = false)
        {
            Verify.IsNotNull(nameof(reader), reader);
            Verify.IsNotEmpty(nameof(name), name);

            int ordinal;
            try
            {
                ordinal = reader.GetOrdinal(name);
            }
            catch (IndexOutOfRangeException)
            {
                if (optional) { return default(T); }
                throw;
            }

            if (reader.IsDBNull(ordinal))
            {
                return default(T);
            }

            string value = reader.GetString(ordinal);

            Type type = typeof(T);
            Type underType = Nullable.GetUnderlyingType(type);

            return (T)Enum.Parse(underType ?? type, value, true);
        }
    }
}
