using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Repeat : NodeCollection<INode>, INode
    {
        public Repeat(string name = null)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"{nameof(Repeat)}, Count={Count}, Children=({this.ToDelimitedString()})";
        }

        public static Repeat operator +(Repeat rootNode, INode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
