using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    /// <summary>
    /// General top level container of nodes
    /// </summary>
    public class RootNode : IEnumerable<INode>, INode, IRule
    {
        private readonly List<INode> _children;

        public RootNode(string name = null)
        {
            _children = new List<INode>();
            Name = name;
        }

        public RootNode(IEnumerable<INode> nodes)
        {
            _children = new List<INode>(nodes);
        }

        /// <summary>
        /// Default empty instance
        /// </summary>
        public static RootNode Empty { get; } = new RootNode();

        public INode this[int index]
        {
            get { return _children[index]; }
            set { _children[index] = value; }
        }

        public string Name { get; }

        public int Count => _children.Count;

        public void Clear()
        {
            _children.Clear();
        }

        public RootNode Add(INode node)
        {
            _children.Add(node);
            return this;
        }

        public RootNode AddRange(IEnumerable<INode> nodes)
        {
            foreach (var node in nodes)
            {
                _children.Add(node);
            }

            return this;
        }

        public IEnumerable<INode> FlattenNodes()
        {
            var stack = new Stack<INode>(this.Reverse<INode>());

            while (stack.Count > 0)
            {
                INode astNode = stack.Pop();

                var childrenNodes = astNode as IEnumerable<INode>;
                if (childrenNodes != null)
                {
                    foreach (var item in childrenNodes)
                    {
                        stack.Push(item);
                    }

                    yield return astNode;
                }

                yield return astNode;
            }
        }

        public IEnumerable<IGrammar<T>> BuildGrammars<T>() where T : System.Enum
        {
            IEnumerable<INode> nodes = FlattenNodes();

            foreach (var node in nodes)
            {
                switch (node)
                {
                    case Body<T> body:
                        yield return new Grammar<T>(body.StartSymbol.TokenType, body.StartSymbol.GrammarMatch);
                        yield return new Grammar<T>(body.EndSymbol.TokenType, body.EndSymbol.GrammarMatch);
                        continue;

                    case Symbol<T> symbol:
                        yield return new Grammar<T>(symbol.TokenType, symbol.GrammarMatch);
                        break;
                }
            }
        }

        public override string ToString()
        {
            return $"{nameof(RootNode)}: Name={Name}, Count={Count}, Children=({this.ToDelimitedString()})";
        }

        public IEnumerator<INode> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        public static RootNode operator +(RootNode self, RootNode node2)
        {
            foreach (var node in node2)
            {
                self.Add(node);
            }

            return self;
        }

        public static RootNode operator +(RootNode rootNode, INode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
