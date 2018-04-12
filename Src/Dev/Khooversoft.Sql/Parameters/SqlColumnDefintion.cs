// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using Microsoft.SqlServer.Server;
using System;
using System.Data;

namespace Khooversoft.Sql
{
    public class SqlColumnDefintion<T>
    {
        public SqlColumnDefintion(string columnName, SqlDbType sqlDbType, Func<T, object> getValue, int? dataSize = null)
        {
            Verify.IsNotEmpty(nameof(columnName), columnName);
            Verify.IsNotNull(nameof(getValue), getValue);

            ColumnName = columnName;
            SqlDbType = sqlDbType;
            GetValue = getValue;
            DataSize = dataSize;
        }

        public string ColumnName { get; }

        public SqlDbType SqlDbType { get; }

        public int? DataSize { get; }

        public Func<T, object> GetValue { get; }

        /// <summary>
        /// Get Sql Metadata for column definition
        /// </summary>
        /// <returns></returns>
        public SqlMetaData GetSqlMetaData()
        {
            switch (this.SqlDbType)
            {
                case SqlDbType.NVarChar:
                    return new SqlMetaData(this.ColumnName, this.SqlDbType, this.DataSize ?? SqlMetaData.Max);

                default:
                    return new SqlMetaData(this.ColumnName, this.SqlDbType);
            }
        }
    }
}
