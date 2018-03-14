using Khooversoft.Toolbox;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Khooversoft.Sql
{
    public class SqlTableParameter<T> : ISqlParameter
    {
        public SqlTableParameter(string parameterName, string tableTypeName)
        {
            Verify.IsNotEmpty(nameof(parameterName), parameterName);
            Verify.IsNotEmpty(nameof(tableTypeName), tableTypeName);

            Name = parameterName;
            TableTypeName = new SqlIdentifier(tableTypeName);
        }

        public SqlTableParameter(string parameterName, SqlIdentifier tableTypeName)
        {
            Verify.IsNotEmpty(nameof(parameterName), parameterName);
            Verify.IsNotNull(nameof(tableTypeName), tableTypeName);

            Name = parameterName;
            TableTypeName = tableTypeName;
        }

        public string Name { get; }

        public SqlIdentifier TableTypeName { get; }

        /// <summary>
        /// Column definitions
        /// </summary>
        public IList<SqlColumnDefintion<T>> ColumnDefinitions { get; } = new List<SqlColumnDefintion<T>>();

        /// <summary>
        /// List of values to write
        /// </summary>
        public List<T> Items { get; } = new List<T>();

        /// <summary>
        /// Return metadata definition from table binding
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SqlMetaData> GetSqlMetadataDefinition()
        {
            foreach (var item in ColumnDefinitions)
            {
                yield return item.GetSqlMetaData();
            }
        }

        /// <summary>
        /// Create SqlParameter for this table type
        /// </summary>
        /// <returns>sql parameter</returns>
        public SqlParameter ToSqlParameter()
        {
            SqlParameter sqlParam = new SqlParameter(Name, ConstructSqlDataRecord(Items));
            sqlParam.SqlDbType = SqlDbType.Structured;
            sqlParam.TypeName = TableTypeName;
            return sqlParam;
        }

        /// <summary>
        /// Construct SQL metadata for table stored procedures
        /// </summary>
        /// <param name="dataItems">list of data items for table type</param>
        /// <returns>Metadata and data for table stored procedures</returns>
        private IEnumerable<SqlDataRecord> ConstructSqlDataRecord(IEnumerable<T> dataItems)
        {
            Verify.IsNotNull(nameof(dataItems), dataItems);

            List<SqlMetaData> metaDataList = new List<SqlMetaData>(GetSqlMetadataDefinition());

            foreach (var item in dataItems)
            {
                SqlDataRecord sdr = new SqlDataRecord(metaDataList.ToArray());

                int index = 0;
                foreach (var rowItem in ColumnDefinitions)
                {
                    sdr.SetValue(index++, rowItem.GetValue(item));
                }

                yield return sdr;
            }
        }
    }
}
