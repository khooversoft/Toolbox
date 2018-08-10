using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public abstract class NodeCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _children = new List<T>();

        public NodeCollection()
        {
        }

        public NodeCollection(IEnumerable<T> nodes)
        {
            _children.AddRange(nodes);
        }

        public T this[int index]
        {
            get { return _children[index]; }
            set { _children[index] = value; }
        }

        public int Count => _children.Count;

        public void Clear()
        {
            _children.Clear();
        }

        public void Add(T node)
        {
            _children.Add(node);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }
}
