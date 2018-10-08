using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class ParserProductionRules<T> : IEnumerable<IRule> where T : System.Enum
    {
        private readonly List<IRule> _rules = new List<IRule>();

        public ParserProductionRules()
        {
        }

        public IRule this[int index]
        {
            get { return _rules[index]; }
            set { _rules[index] = value; }
        }

        public int Count => _rules.Count;

        public RootNode AstNode => new RootNode(_rules.OfType<RootNode>());   

        public IEnumerable<Bracket<T>> Brackets => _rules.OfType<Bracket<T>>();

        public IEnumerable<IGrammar<T>> Grammars => _rules.OfType<IGrammar<T>>();

        public void Clear()
        {
            _rules.Clear();
        }

        public ParserProductionRules<T> Add(IRule node)
        {
            _rules.Add(node);
            return this;
        }

        public IEnumerator<IRule> GetEnumerator()
        {
            return _rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rules.GetEnumerator();
        }

        public static ParserProductionRules<T> operator +(ParserProductionRules<T> prNode, IRule nodeToAdd)
        {
            prNode.Add(nodeToAdd);
            return prNode;
        }

        public static ParserProductionRules<T> operator +(ParserProductionRules<T> prNode, Bracket<T> nodeToAdd)
        {
            prNode.Add(nodeToAdd);
            return prNode;
        }
    }
}
