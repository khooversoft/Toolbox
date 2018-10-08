using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Optional : NodeCollection<INode>, INode
    {
        public Optional(string name = null)
        {
            Name = name;
        }

        public Optional(IEnumerable<INode> nodes)
            : base(nodes)
        {
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"{nameof(Optional)}, Name={Name}, Count={Count}, Children=({this.ToDelimitedString()})";
        }

        public static Optional operator +(Optional rootNode, INode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
