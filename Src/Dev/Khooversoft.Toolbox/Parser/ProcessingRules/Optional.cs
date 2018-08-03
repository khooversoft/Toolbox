using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Optional : AstNodeCollection<IAstNode>, IAstNode
    {
        public Optional(string name = null)
        {
            Name = name;
        }

        public Optional(IEnumerable<IAstNode> nodes)
            : base(nodes)
        {
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"{nameof(Optional)}, Name={Name}, Count={Count}, Children=({this.ToDelimitedString()})";
        }

        public static Optional operator +(Optional rootNode, IAstNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
