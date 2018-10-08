using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public interface IToken
    {
    }

    public interface IToken<T> : IToken where T : System.Enum
    {
        T GrammarType { get; }
    }
}
