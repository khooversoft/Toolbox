using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Parser
{
    public class Skip<T> : AstNodeCollection<IAstNode>, IAstNode
    {
        public Skip(bool supportNested = true, string name = null)
        {
            SupportNested = supportNested;
            Name = name;
        }

        public bool SupportNested { get; }

        public string Name { get; }

        public override string ToString()
        {
            return $"{nameof(Skip<T>)}, SupportNested={SupportNested}, Count={Count}, Children=({this.ToDelimitedString()})";
        }

        public static Skip<T> operator +(Skip<T> rootNode, IAstNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
