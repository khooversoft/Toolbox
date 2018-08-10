using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class ParserResult
    {
        public ParserResult(RootNode rootNode, IEnumerable<IToken> tokens)
        {
            RootNode = rootNode;
            Tokens = tokens.ToList();
        }

        public ParserResult(IEnumerable<IToken> tokens, RootNode lastGood, IEnumerable<INode> outstandingNodes = null)
        {
            Tokens = tokens?.ToList();
            LastGood = lastGood;
            OutstandingNodes = outstandingNodes?.ToList();
        }

        public RootNode RootNode { get; }

        public IReadOnlyList<IToken> Tokens { get; }

        public IReadOnlyList<INode> OutstandingNodes { get; }

        public RootNode LastGood { get; }

        public bool IsSuccess => RootNode != null;
    }
}
