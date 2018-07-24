using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Parser
{
    public class Bracket<T> : IRule where T : System.Enum
    {
        public Bracket(Symbol<T> startSymbol, Symbol<T> endSymbol)
        {
            StartSymbol = startSymbol;
            EndSymbol = endSymbol;
        }

        public Symbol<T> StartSymbol { get; }

        public Symbol<T> EndSymbol { get; }

        public IEnumerable<IGrammar<T>> Grammars
        {
            get
            {
                yield return new Grammar<T>(StartSymbol.TokenType, StartSymbol.GrammarMatch);
                yield return new Grammar<T>(EndSymbol.TokenType, EndSymbol.GrammarMatch);
            }
        }

        public AstNode IfBracket(Token<T> token)
        {
            bool isStartSymbol = token?.GrammarType.Equals(StartSymbol.TokenType) == true;
            bool isEndSymbol = token?.GrammarType.Equals(EndSymbol.TokenType) == true;

            if (!isStartSymbol && !isEndSymbol)
            {
                return null;
            }

            AstNode astNodes = new AstNode();
            astNodes += new Symbol<T>(token.GrammarType);
            return astNodes;
        }

        public override string ToString()
        {
            return $"{nameof(Bracket<T>)}: StartSymbol={StartSymbol}, EndSymbol={EndSymbol}";
        }

        public override bool Equals(object obj)
        {
            Bracket<T> context = obj as Bracket<T>;
            return StartSymbol.Equals(context?.StartSymbol) && EndSymbol.Equals(context?.EndSymbol);
        }

        public override int GetHashCode()
        {
            return StartSymbol.GetHashCode() ^ EndSymbol.GetHashCode();
        }
    }
}
