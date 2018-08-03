using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class AstProductionRules<T> : IEnumerable<IRule> where T : System.Enum
    {
        private readonly List<IRule> _rules = new List<IRule>();

        public AstProductionRules()
        {
        }

        public IRule this[int index]
        {
            get { return _rules[index]; }
            set { _rules[index] = value; }
        }

        public int Count => _rules.Count;

        public AstNode AstNode => new AstNode(_rules.OfType<AstNode>());   

        public IEnumerable<Bracket<T>> Brackets => _rules.OfType<Bracket<T>>();

        public IEnumerable<IGrammar<T>> Grammars => _rules.OfType<IGrammar<T>>();

        public void Clear()
        {
            _rules.Clear();
        }

        public AstProductionRules<T> Add(IRule node)
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

        public static AstProductionRules<T> operator +(AstProductionRules<T> prNode, IRule nodeToAdd)
        {
            prNode.Add(nodeToAdd);
            return prNode;
        }

        public static AstProductionRules<T> operator +(AstProductionRules<T> prNode, Bracket<T> nodeToAdd)
        {
            prNode.Add(nodeToAdd);
            return prNode;
        }
    }
}
