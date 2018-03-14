using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Sql
{
    public interface ISqlParameter
    {
        SqlParameter ToSqlParameter();
    }
}
