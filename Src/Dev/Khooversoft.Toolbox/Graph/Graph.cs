using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Basic graph support for user extended node and edge types.  Directed edges are supported
    /// including the ability to support non-directed edges by using 2 directed edges, each in
    /// both directions.
    /// 
    /// Type 'int' is used for node id for efficiency in storage and processing.
    /// 
    /// </summary>
    /// <typeparam name="TVertex">vertex type</typeparam>
    /// <typeparam name="TEdge">edge type</typeparam>
    public class Graph<TVertex, TEdge> : GraphStorage<TVertex, TEdge>
        where TVertex : Vertex
        where TEdge : DirectedEdge
    {
        private readonly GraphStorage<TVertex, TEdge> _storage = new GraphStorage<TVertex, TEdge>();

        public Graph()
        {
        }

        public new Graph<TVertex, TEdge> Add(TVertex vertex)
        {
            base.Add(vertex);
            return this;
        }

        public new Graph<TVertex, TEdge> Add(TEdge edge)
        {
            base.Add(edge);
            return this;
        }

        public IEnumerable<TEdge> GedEdges(int nodeId)
        {
            return _edges.Values
                .Where(x => x.IsSameNode(nodeId));
        }

        public IEnumerable<TEdge> GetDirectedEdges(int nodeId)
        {
            return _edges.Values
                .Where(x => x.EdgeId.SourceNodeId == nodeId);
        }

        public IReadOnlyList<IReadOnlyList<int>> GetTopologicalOrdering()
        {
            var list = new List<IReadOnlyList<int>>();

            var first = _vertices.Values
                .Where(x => !_edges.Values.Any(y => y.EdgeId.ToNodeId == x.NodeId))
                .Select(x => x.NodeId);

            if( !first.Any())
            {
                return list;
            }

            list.Add(new List<int>(first));
            int index = 0;

            while (true)
            {
                var add = _edges.Values
                    .Join(list[index], x => x.EdgeId.SourceNodeId, x => x, (o, i) => o)
                    .Select(x => x.EdgeId.ToNodeId);

                if(!add.Any())
                {
                    break;
                }

                list.Add(new List<int>(add));
                index++;
            }

            return list;
        }
    }
}
