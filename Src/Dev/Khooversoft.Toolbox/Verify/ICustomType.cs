using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Verify custom type
    /// </summary>
    /// <typeparam name="T">type</typeparam>
    public interface ICustomType<T>
    {
        bool IsValueValid();
    }
}
