using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test
{
    [Trait("Category", "Toolbox")]
    public class GraphTests
    {
        [Fact]
        public void SimpleFailTest()
        {
            var graph = new Graph<Vertex, DirectedEdge>();

            graph.Add(new Vertex(1));
            graph.Invoking(x => x.Add(new Vertex(1))).Should().Throw<ArgumentException>();
            graph.Invoking(x => x.Add(new DirectedEdge(0, 1))).Should().Throw<ArgumentException>();
            graph.Invoking(x => x.Add(new DirectedEdge(1, 0))).Should().Throw<ArgumentException>();
            graph.Remove(0).Should().BeFalse();

            graph.Remove(1);
        }

        [Fact]
        public void BaseStateGraphTest()
        {
            var graph = new Graph<Vertex, DirectedEdge>();

            graph.GedEdges(0).Count().Should().Be(0);
            graph.GedEdges(1).Count().Should().Be(0);
        }

        [Fact]
        public void GraphSimpleTest()
        {
            var graph = new Graph<Vertex, DirectedEdge>()
                .Add(new Vertex(0))
                .Add(new Vertex(1))
                .Add(new DirectedEdge(0, 1));

            graph.Invoking(x => x.Add(new DirectedEdge(0, 1))).Should().Throw<ArgumentException>();
            graph.GedEdges(0).Count().Should().Be(1);
            graph.GedEdges(1).Count().Should().Be(1);
            graph.GedEdges(2).Count().Should().Be(0);

            graph.Remove(new EdgeId(0, 1)).Should().BeTrue();
            graph.Remove(new EdgeId(0, 1)).Should().BeFalse();
            graph.Remove(0).Should().BeTrue();
            graph.Remove(1).Should().BeTrue();
            graph.Remove(1).Should().BeFalse();
        }

        [Fact]
        public void MultipleEdgesTest()
        {
            var graph = new Graph<Vertex, DirectedEdge>()
                .Add(new Vertex(0))
                .Add(new Vertex(1))
                .Add(new DirectedEdge(0, 1))
                .Add(new DirectedEdge(1, 0));

            graph.GedEdges(0).Count().Should().Be(2);
            graph.GedEdges(1).Count().Should().Be(2);
            graph.GedEdges(2).Count().Should().Be(0);
        }

        [Fact]
        public void TreeTest()
        {
            // Tree graph
            // 0 -> 1 -> 2
            // 0 -> 3
            var graph = new Graph<Vertex, DirectedEdge>()
                .Add(new Vertex(0))
                .Add(new Vertex(1))
                .Add(new Vertex(2))
                .Add(new Vertex(3))
                .Add(new DirectedEdge(0, 1))
                .Add(new DirectedEdge(1, 2))
                .Add(new DirectedEdge(0, 3));

            // Node 0
            graph.GedEdges(0).OrderBy(x => x.EdgeId)
                .Zip(new EdgeId[] { new EdgeId(0, 1), new EdgeId(0, 3) }, (o, i) => new { o = o.EdgeId, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();

            graph.GetDirectedEdges(0).OrderBy(x => x.EdgeId)
                .Zip(new EdgeId[] { new EdgeId(0, 1), new EdgeId(0, 3) }, (o, i) => new { o = o.EdgeId, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();

            // Node 1
            graph.GedEdges(1).OrderBy(x => x.EdgeId)
                .Zip(new EdgeId[] { new EdgeId(0, 1), new EdgeId(1, 2) }, (o, i) => new { o = o.EdgeId, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();

            graph.GetDirectedEdges(1).OrderBy(x => x.EdgeId)
                .Zip(new EdgeId[] { new EdgeId(1, 2) }, (o, i) => new { o = o.EdgeId, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();

            // Node 2
            graph.GedEdges(2).OrderBy(x => x.EdgeId)
                .Zip(new EdgeId[] { new EdgeId(1, 2) }, (o, i) => new { o = o.EdgeId, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();

            graph.GetDirectedEdges(2).Count().Should().Be(0);

            // Node 3
            graph.GedEdges(3).OrderBy(x => x.EdgeId)
                .Zip(new EdgeId[] { new EdgeId(0, 3) }, (o, i) => new { o = o.EdgeId, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();

            graph.GetDirectedEdges(3).Count().Should().Be(0);
        }
    }
}
