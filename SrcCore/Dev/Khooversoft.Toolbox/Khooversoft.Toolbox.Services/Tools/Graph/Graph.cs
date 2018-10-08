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

        /// <summary>
        /// Get topological order from graph tree.  This is done by processing
        /// tree directed edges to understand dependencies.
        /// </summary>
        /// <param name="numberOfLevels">only process n levels is specified</param>
        /// <param name="processedNodes">Nodes that have already been processed</param>
        /// <returns>list of list of vertices for each level</returns>
        public IReadOnlyList<IReadOnlyList<int>> GetTopologicalOrdering(int? numberOfLevels = null, IEnumerable<int> processedNodes = null)
        {
            var list = new List<IReadOnlyList<int>>();
            GraphStorage<TVertex, TEdge> processGraph = base.Clone();

            (processedNodes ?? Enumerable.Empty<int>()).Run(x => processGraph.Remove(x));

            var nodesProcessed = new HashSet<int>(processedNodes ?? Enumerable.Empty<int>());

            while (numberOfLevels != list.Count)
            {
                var freeNode = processGraph.Vertices.Values
                    .Where(x => !processGraph.Edges.Values.Any(y => y.EdgeId.ToNodeId == x.NodeId))
                    .Select(x => x.NodeId)
                    .ToList();

                if (freeNode.Count == 0)
                {
                    return list;
                }

                list.Add(new List<int>(freeNode));
                freeNode.Run(x => processGraph.Remove(x));
            }

            return list;
        }
    }
}
