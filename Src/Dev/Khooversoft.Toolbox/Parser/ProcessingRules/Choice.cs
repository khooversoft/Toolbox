using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Choice : NodeCollection<INode>, INode
    {
        public Choice(string name = null)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"{nameof(Choice)}, Count={Count}, Children=({this.ToDelimitedString()})";
        }

        public static Choice operator +(Choice rootNode, INode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
