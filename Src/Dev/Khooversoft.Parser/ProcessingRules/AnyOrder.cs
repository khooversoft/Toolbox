using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Parser
{
    public class AnyOrder : AstNodeCollection<IAstNode>, IAstNode
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

        public static AnyOrder operator +(AnyOrder rootNode, IAstNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}