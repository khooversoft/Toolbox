using System.Collections.Generic;

namespace Khooversoft.Security
{
    public interface IHmacConfiguration
    {
        IEnumerable<string> Headers { get; }
    }
}
