using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class AnyOrder : NodeCollection<INode>, INode
    {
        public AnyOrder(string name = null)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"{nameof(Choice)}, Count={Count}";
        }

        public static AnyOrder operator +(AnyOrder rootNode, INode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}