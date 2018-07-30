using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public class DirectedEdge : IGraphComponent
    {
        public DirectedEdge(EdgeId edigeId)
        {
            EdgeId = edigeId;
        }

        public DirectedEdge(int sourceNodeId, int toNodeId)
        {
            EdgeId = new EdgeId(sourceNodeId, toNodeId);
        }

        public EdgeId EdgeId { get; }

        public bool IsSameEdge(DirectedEdge directedEdge)
        {
            return (EdgeId.SourceNodeId == directedEdge.EdgeId.SourceNodeId && EdgeId.ToNodeId == directedEdge.EdgeId.ToNodeId)
                || (EdgeId.ToNodeId == directedEdge.EdgeId.SourceNodeId && EdgeId.SourceNodeId == directedEdge.EdgeId.ToNodeId);
        }

        public bool IsSameNode(int nodeId) => EdgeId.SourceNodeId == nodeId || EdgeId.ToNodeId == nodeId;

        public static long CreateEdgeId(int sourceNodeId, int toNodeId) => (long)sourceNodeId << 32 | (uint)toNodeId;

        public override string ToString()
        {
            return $"EdgeId={EdgeId}";
        }
    }
}
