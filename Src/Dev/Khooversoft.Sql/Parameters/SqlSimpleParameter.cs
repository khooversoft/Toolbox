using Khooversoft.Toolbox;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Khooversoft.Sql
{
    [DebuggerDisplay("ParameterName={ParameterName}, Value={Value}")]
    public class SqlSimpleParameter : ISqlParameter
    {
        public SqlSimpleParameter(string name, object value)
        {
            Verify.IsNotEmpty(nameof(name), name);
            Verify.IsNotNull(nameof(value), value);

            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }

        public SqlParameter ToSqlParameter()
        {
            return new SqlParameter(Name, Value);
        }
    }
}
