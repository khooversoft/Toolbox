using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Token<T> : IToken<T> where T : System.Enum
    {
        public Token(T grammarType)
        {
            GrammarType = grammarType;
        }

        public T GrammarType { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Token<T>))
            {
                return false;
            }

            return ((Token<T>)obj).GrammarType.Equals(GrammarType);
        }

        public override int GetHashCode() => GrammarType.GetHashCode();

        public override string ToString() => $"{nameof(Token<T>)}: Grammar Type:{GrammarType}";
    }
}
