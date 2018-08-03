using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    internal class BracketManager<T> : IEnumerable<Bracket<T>> where T : System.Enum
    {
        private readonly List<Bracket<T>> _bracketList;

        public BracketManager(IEnumerable<Bracket<T>> brackets)
        {
            _bracketList = brackets?.ToList();
        }

        public void SetBracket(Symbol<T> startSymbol, Symbol<T> endSymbol)
        {
            _bracketList.Add(new Bracket<T>(startSymbol, endSymbol));
        }

        public AstNode ProcessBracket(Token<T> token)
        {
            foreach (var item in _bracketList ?? Enumerable.Empty<Bracket<T>>())
            {
                AstNode astNodes = item.IfBracket(token);
                if (astNodes != null)
                {
                    return astNodes;
                }
            }

            return null;
        }

        public bool IsStartBracket(Token<T> token)
        {
            return _bracketList.Any(x => x.StartSymbol.TokenType.Equals(token.GrammarType));
        }

        public bool IsEndBracket(Token<T> token)
        {
            return _bracketList.Any(x => x.EndSymbol.TokenType.Equals(token.GrammarType));
        }

        public override string ToString()
        {
            return $"{nameof(BracketManager<T>)}: _bracketList.Count={_bracketList.Count}";
        }

        public IEnumerator<Bracket<T>> GetEnumerator()
        {
            return _bracketList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _bracketList.GetEnumerator();
        }
    }
}
