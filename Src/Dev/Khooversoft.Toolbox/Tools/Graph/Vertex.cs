using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public class Vertex : IGraphComponent
    {
        public Vertex(int nodeId)
        {
            NodeId = nodeId;
        }

        public int NodeId { get; }

        public override string ToString()
        {
            return $"NodeId={NodeId}";
        }
    }
}
