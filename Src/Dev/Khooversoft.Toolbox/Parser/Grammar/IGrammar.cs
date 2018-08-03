using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public interface IGrammar<T> : IRule where T : System.Enum
    {
        T GrammarType { get; }

        string Match { get; }

        IToken CreateToken();
    }
}
