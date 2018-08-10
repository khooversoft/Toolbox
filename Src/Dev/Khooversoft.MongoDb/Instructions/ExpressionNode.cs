//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MongoDB.Bson;

//namespace Khooversoft.MongoDb
//{
//    public class ExpressionNode<T> : IExpressionNode, IEnumerable<IExpressionNode>
//    {
//        private readonly List<IExpressionNode> _children = new List<IExpressionNode>();

//        public ExpressionNode(T type)
//        {
//            Type = type;
//        }

//        public ExpressionNode(T type, string name)
//            : this(type)
//        {
//            Name = name;
//        }

//        public ExpressionNode(T type, string name, IEnumerable<IExpressionNode> nodes)
//            : this(type, name)
//        {
//            _children.AddRange(nodes);
//        }

//        public T Type { get; }

//        public string Name { get; }

//        public IExpressionNode this[int index]
//        {
//            get { return _children[index]; }
//            set { _children[index] = value; }
//        }

//        public int Count => _children.Count;

//        public void Clear()
//        {
//            _children.Clear();
//        }

//        public void Add(IExpressionNode node)
//        {
//            _children.Add(node);
//        }

//        public void RemoveAt(int index)
//        {
//            _children.RemoveAt(index);
//        }

//        public BsonDocument ToBsonDocument()
//        {
//            var document = new BsonDocument();

//            foreach (var item in this)
//            {
//                switch (item)
//                {
//                    case TerminalNode<T> terminal:
//                        document.AddRange(terminal.ToBsonDocument());
//                        break;

//                    case ExpressionNode<T> expression:
//                        BsonDocument queryDocument = expression.ToBsonDocument();
//                        var array = new BsonArray();

//                        //foreach(var docItem in queryDocument)
//                        //{

//                        //}

//                        document.Add(expression.Name, new BsonArray { queryDocument });
//                        break;

//                    default:
//                        throw new ArgumentException($"Unknown type-{item.GetType().FullName}");
//                }
//            }

//            return document;
//        }

//        public IEnumerator<IExpressionNode> GetEnumerator()
//        {
//            return _children.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return _children.GetEnumerator();
//        }

//        public static ExpressionNode<T> operator +(ExpressionNode<T> rootNode, IExpressionNode nodeToAdd)
//        {
//            rootNode.Add(nodeToAdd);
//            return rootNode;
//        }
//    }
//}
