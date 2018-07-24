using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Parser
{
    public class ParserResult
    {
        public ParserResult(AstNode rootNode, IEnumerable<IToken> tokens)
        {
            RootNode = rootNode;
            Tokens = tokens.ToList();
        }

        public ParserResult(IEnumerable<IToken> tokens, AstNode lastGood, IEnumerable<IAstNode> outstandingNodes = null)
        {
            Tokens = tokens?.ToList();
            LastGood = lastGood;
            OutstandingNodes = outstandingNodes?.ToList();
        }

        public AstNode RootNode { get; }

        public IReadOnlyList<IToken> Tokens { get; }

        public IReadOnlyList<IAstNode> OutstandingNodes { get; }

        public AstNode LastGood { get; }

        public bool IsSuccess => RootNode != null;
    }
}
