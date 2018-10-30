using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public abstract class InstructionCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _list = new List<T>();

        protected InstructionCollection()
        {
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public void Add(T node) => _list.Add(node);

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public static InstructionCollection<T> operator +(InstructionCollection<T> rootNode, T nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
