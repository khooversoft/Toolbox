using Khooversoft.Toolbox;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Khooversoft.Sql
{
    /// <summary>
    /// Handle simple SQL parameters (immutable)
    /// </summary>
    [DebuggerDisplay("Name={Name}, Value={Value}")]
    public class SqlSimpleParameter : ISqlParameter
    {
        public SqlSimpleParameter(string name, object value)
        {
            Verify.IsNotEmpty(nameof(name), name);
            Verify.IsNotNull(nameof(value), value);

            Name = name;
            Value = value;
        }

        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of the parameter
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Convert to SQL Parameter
        /// </summary>
        /// <returns>SQL parameter</returns>
        public SqlParameter ToSqlParameter()
        {
            return new SqlParameter(Name, Value);
        }
    }
}
