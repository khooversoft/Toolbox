using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Graph
{
    [Trait("Category", "Toolbox")]
    public class DependencyTreeTests
    {
        [Fact]
        public void EmptyTreeDependencyTree()
        {
            var g1 = new Graph<Vertex, DirectedEdge>();
            g1.GetTopologicalOrdering().Count.Should().Be(0);

            // Tree graph
            // 0 -> 1 -> 2
            // 0 -> 3
            var graph = new Graph<Vertex, DirectedEdge>
            {
                new Vertex(0),
            };

            graph.GetTopologicalOrdering().Count.Should().Be(1);
        }

        [Fact]
        public void TreeDependencySingleForTree()
        {
            // Tree graph
            // 0 -> 1
            var graph = new Graph<Vertex, DirectedEdge>
            {
                new Vertex(0),
                new Vertex(1),
                new Vertex(2),
                new Vertex(3),
                new DirectedEdge(0, 1),
            };

            var x1 = graph.GetTopologicalOrdering();

            var expectedResults = new List<int[]>
            {
                new int[] { 0, 2, 3 },
                new int[] { 1 },
            };

            x1.Count.Should().Be(expectedResults.Count);

            var x2 = x1
                .Zip(expectedResults, (o, i) => new { o = o.OrderBy(x => x), i });

            var x3 = x2
                .All(x => x.o.SequenceEqual(x.i))
                .Should().BeTrue();
        }

        [Fact]
        public void TreeDependencyOrderForTree()
        {
            // Tree graph
            // 0 -> 1 -> 2
            // 0 -> 3
            var graph = new Graph<Vertex, DirectedEdge>
            {
                new Vertex(0),
                new Vertex(1),
                new Vertex(2),
                new Vertex(3),
                new DirectedEdge(0, 1),
                new DirectedEdge(1, 2),
                new DirectedEdge(0, 3),
            };

            var x1 = graph.GetTopologicalOrdering();

            var expectedResults = new List<int[]>
            {
                new int[] { 0 },
                new int[] { 1, 3 },
                new int[] { 2 }
            };

            x1.Count.Should().Be(expectedResults.Count);

            var x2 = x1
                .Zip(expectedResults, (o, i) => new { o = o.OrderBy(x => x), i });

            var x3 = x2
                .All(x => x.o.SequenceEqual(x.i))
                .Should().BeTrue();
        }

        [Fact]
        public void TreeDependencyOrder_2_Tree()
        {
            // Tree graph
            // 0 -> 1 -> 2 -> 3
            //      1 -> 4
            var graph = new Graph<Vertex, DirectedEdge>
            {
                new Vertex(0),
                new Vertex(1),
                new Vertex(2),
                new Vertex(3),
                new Vertex(4),
                new DirectedEdge(0, 1),
                new DirectedEdge(1, 2),
                new DirectedEdge(2, 3),
                new DirectedEdge(1, 4),
            };

            var x1 = graph.GetTopologicalOrdering();

            var expectedResults = new List<int[]>
            {
                new int[] { 0 },
                new int[] { 1 },
                new int[] { 2, 4 },
                new int[] { 3 },
            };

            x1.Count.Should().Be(expectedResults.Count);

            var x2 = x1
                .Zip(expectedResults, (o, i) => new { o = o.OrderBy(x => x), i });

            var x3 = x2
                .All(x => x.o.SequenceEqual(x.i))
                .Should().BeTrue();
        }
    }
}
