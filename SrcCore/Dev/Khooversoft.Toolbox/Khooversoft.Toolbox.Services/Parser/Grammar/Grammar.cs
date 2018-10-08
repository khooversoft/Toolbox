using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Grammar<T> : IGrammar<T>, IRule where T : System.Enum
    {
        public Grammar(T grammarType, string match)
        {
            GrammarType = grammarType;
            Match = match;
        }

        public T GrammarType { get; }

        public string Match { get; }

        public IToken CreateToken()
        {
            return new Token<T>(GrammarType);
        }

        public override string ToString()
        {
            return $"{nameof(Grammar<T>)}: GrammarType={GrammarType}, Match={Match}";
        }
    }
}
