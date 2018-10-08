// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Khooversoft.Toolbox.Sql
{
    /// <summary>
    /// Extensions for supporting reading and converting SQL data to .NET types
    /// </summary>
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
            catch (IndexOutOfRangeException ex)
            {
                if (optional)
                {
                    return default(T);
                }

                throw new IndexOutOfRangeException($"{name} is not in the SqlDataReader", ex);
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
        /// Parse column data in SQL reader to enum type.
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <param name="reader">SQL reader</param>
        /// <param name="name">column name</param>
        /// <param name="optional">if true, the value is optional</param>
        /// <returns>null or enum value</returns>
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

        /// <summary>
        /// Get collections from SQL reader.  SQL reader support reading multiple data sets.
        /// This capability allows a single call to a stored procedure to return different data sets
        /// as required, improving performance and providing single snapshot view of related data.
        /// 
        /// This extension is called for every dataset returned by a single call to a stored procedures.
        /// </summary>
        /// <typeparam name="T">type of collection</typeparam>
        /// <param name="reader">SQL reader</param>
        /// <param name="context">work context</param>
        /// <param name="factory">type constructor factory</param>
        /// <returns>List of types returned by SQL.  List can be 0 in length if no data was returned for dataset.</returns>
        public static IList<T> GetCollection<T>(this SqlDataReader reader, IWorkContext context, Func<IWorkContext, SqlDataReader, T> factory)
        {
            var list = new List<T>();

            while (reader.Read())
            {
                list.Add(factory(context, reader));
            }

            reader.NextResult();

            return list;
        }
    }
}
