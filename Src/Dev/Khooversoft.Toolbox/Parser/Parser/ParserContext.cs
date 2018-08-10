using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    internal class ParserContext
    {
        public ParserContext(IEnumerable<IToken> tokens)
        {
            tokens = tokens ?? throw new ArgumentException(nameof(tokens));
            InputTokens = new CursorList<IToken>(tokens);
        }

        public CursorList<IToken> InputTokens { get; }

        public IReadOnlyList<INode> OutstandingNodes { get; set; }

        public RootNode LastGood { get; set; }
    }
}
