using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Parser
{
    public class Choice : AstNodeCollection<IAstNode>, IAstNode
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

        public static Choice operator +(Choice rootNode, IAstNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
