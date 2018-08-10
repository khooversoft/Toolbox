using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public abstract class InstructionCollection : IEnumerable<IInstructionNode>
    {
        private readonly List<IInstructionNode> _list = new List<IInstructionNode>();

        protected InstructionCollection()
        {
        }

        public IInstructionNode this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public void Add(IInstructionNode node) => _list.Add(node);

        public IEnumerator<IInstructionNode> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public static InstructionCollection operator +(InstructionCollection rootNode, IInstructionNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
