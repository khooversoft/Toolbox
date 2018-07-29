using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public class GraphStorage<TVertex, TEdge> : IEnumerable<IGraphComponent>
        where TVertex : Vertex
        where TEdge : DirectedEdge
    {
        protected readonly Dictionary<int, TVertex> _vertices = new Dictionary<int, TVertex>();
        protected readonly Dictionary<long, TEdge> _edges = new Dictionary<long, TEdge>();

        public GraphStorage()
        {
        }

        public IReadOnlyDictionary<int, TVertex> Vertices => _vertices;

        public IReadOnlyDictionary<long, TEdge> Edges => _edges;

        public void Add(IGraphComponent component)
        {
            Add(component);
        }

        public void Add(TVertex vertex)
        {
            _vertices.Add(vertex.NodeId, vertex);
        }

        public void Add(TEdge edge)
        {
            if (!_vertices.ContainsKey(edge.EdgeId.SourceNodeId) || !_vertices.ContainsKey(edge.EdgeId.ToNodeId))
            {
                throw new ArgumentException("Source or to node id does not exist in Vertices");
            }

            _edges.Add(edge.EdgeId, edge);
        }

        public bool Remove(int nodeId)
        {
            bool result = _vertices.Remove(nodeId);
            var list = _edges.Values.Where(x => x.IsSameNode(nodeId));

            foreach (var item in list)
            {
                _edges.Remove(item.EdgeId);
            }

            return result;
        }

        public bool Remove(EdgeId edgeId)
        {
            return _edges.Remove(edgeId);
        }

        public int RemoveEdges(int nodeId)
        {
            var list = _edges.Values.Where(x => x.IsSameNode(nodeId));

            int count = 0;
            foreach (var item in list)
            {
                if (_edges.Remove(item.EdgeId))
                {
                    count++;
                }
            }

            return count;
        }

        public IEnumerator<IGraphComponent> GetEnumerator()
        {
            return _vertices.Values
                .OfType<IGraphComponent>()
                .Concat(_edges.Values.OfType<IGraphComponent>())
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
