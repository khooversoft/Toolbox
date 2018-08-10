using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Symbol<T> : INode where T : System.Enum
    {
        public Symbol(T tokenType)
        {
            TokenType = tokenType;
            GrammarMatch = null;
        }

        public Symbol(T tokenType, string grammarMatch)
        {
            TokenType = tokenType;
            GrammarMatch = grammarMatch;
        }

        public string Name => nameof(Symbol<T>);

        public T TokenType { get; }

        public string GrammarMatch { get; }

        public Grammar<T> ToGrammer()
        {
            return new Grammar<T>(TokenType, GrammarMatch);
        }

        public override bool Equals(object obj)
        {
            var exp = obj as Symbol<T>;
            if (exp == null)
            {
                return false;
            }

            return TokenType.Equals(exp.TokenType);
        }

        public override int GetHashCode()
        {
            return TokenType.GetHashCode();
        }

        public override string ToString()
        {
            if( string.IsNullOrWhiteSpace(GrammarMatch))
            {
                return $"{TokenType}";
            }

            return $"{nameof(Symbol<T>)}: TokenType={TokenType}, GrammarMatch={GrammarMatch}";
        }
    }
}
