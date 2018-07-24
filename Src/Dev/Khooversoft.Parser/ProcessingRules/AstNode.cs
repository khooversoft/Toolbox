using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Parser
{
    /// <summary>
    /// General top level container of nodes
    /// </summary>
    public class AstNode : IEnumerable<IAstNode>, IAstNode, IRule
    {
        private readonly List<IAstNode> _children;

        public AstNode(string name = null)
        {
            _children = new List<IAstNode>();
            Name = name;
        }

        public AstNode(IEnumerable<IAstNode> nodes)
        {
            _children = new List<IAstNode>(nodes);
        }

        /// <summary>
        /// Default empty instance
        /// </summary>
        public static AstNode Empty { get; } = new AstNode();

        public IAstNode this[int index]
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

        public AstNode Add(IAstNode node)
        {
            _children.Add(node);
            return this;
        }

        public AstNode AddRange(IEnumerable<IAstNode> nodes)
        {
            foreach (var node in nodes)
            {
                _children.Add(node);
            }

            return this;
        }

        public IEnumerable<IAstNode> FlattenNodes()
        {
            var stack = new Stack<IAstNode>(this.Reverse<IAstNode>());

            while (stack.Count > 0)
            {
                IAstNode astNode = stack.Pop();

                var childrenNodes = astNode as IEnumerable<IAstNode>;
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

        public IEnumerable<IToken> FlattenTokens()
        {
            var stack = new Stack<IToken>(this.OfType<IEnumerable<IToken>>().SelectMany(x => x).Reverse());

            while (stack.Count > 0)
            {
                IToken astNode = stack.Pop();

                var childrenNodes = astNode as IEnumerable<IToken>;
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
            IEnumerable<IAstNode> nodes = FlattenNodes();

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
            return $"{nameof(AstNode)}: Name={Name}, Count={Count}, Children=({this.ToDelimitedString()})";
        }

        public IEnumerator<IAstNode> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        public static AstNode operator +(AstNode self, AstNode node2)
        {
            foreach (var node in node2)
            {
                self.Add(node);
            }

            return self;
        }

        public static AstNode operator +(AstNode rootNode, IAstNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
