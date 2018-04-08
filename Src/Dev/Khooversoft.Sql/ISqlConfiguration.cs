using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Sql
{
    /// <summary>
    /// Interface for SQL configuration
    /// </summary>
    public interface ISqlConfiguration
    {
        string ConnectionString { get; }
    }
}
