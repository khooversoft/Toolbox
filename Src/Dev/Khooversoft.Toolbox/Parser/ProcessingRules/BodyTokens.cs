using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class BodyTokens : IEnumerable<IToken>, INode
    {
        private readonly List<IToken> _tokens;

        public BodyTokens(INode referenceNode, IEnumerable<IToken> tokens)
        {
            ReferencedNode = referenceNode;
            _tokens = tokens?.ToList();
        }

        public string Name => "BodyTokens";

        public INode ReferencedNode { get; }

        public IToken this[int index] => _tokens[index];

        public int Count => _tokens.Count;

        public IEnumerator<IToken> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        public override string ToString()
        {
            return $"{nameof(BodyTokens)}: Count={Count}";
        }
    }
}
