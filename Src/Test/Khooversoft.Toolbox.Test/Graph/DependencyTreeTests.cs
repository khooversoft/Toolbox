using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var expectedResults = new List<int[]>
            {
                new int[] { 0, 2, 3 },
                new int[] { 1 },
            };

            RunTest(graph, expectedResults);
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

            var expectedResults = new List<int[]>
            {
                new int[] { 0 },
                new int[] { 1, 3 },
                new int[] { 2 }
            };

            RunTest(graph, expectedResults);
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

            var expectedResults = new List<int[]>
            {
                new int[] { 0 },
                new int[] { 1 },
                new int[] { 2, 4 },
                new int[] { 3 },
            };

            RunTest(graph, expectedResults);
        }

        [Fact]
        public void TreeDependencyOrder_3_Tree()
        {
            // Tree graph
            // 0 -> 1 -> 2 -> 3
            //      1 -> 4 -> 3
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
                new DirectedEdge(4, 3),
            };

            var expectedResults = new List<int[]>
            {
                new int[] { 0 },
                new int[] { 1 },
                new int[] { 2, 4 },
                new int[] { 3 },
            };

            RunTest(graph, expectedResults);
        }

        [Fact]
        public void TreeDependencyOrder_4_Tree()
        {
            // Tree graph
            // 0 -> 1 -> 2 -> 3
            //      1 -> 3
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
                new DirectedEdge(1, 3),
            };

            var expectedResults = new List<int[]>
            {
                new int[] { 0, 4 },
                new int[] { 1 },
                new int[] { 2 },
                new int[] { 3 },
            };

            RunTest(graph, expectedResults);
        }

        [Fact]
        public void TreeDependencyOrder_5_Tree()
        {
            // Tree graph
            // 0 -> 1 -> 2 -> 3
            //      1 -> 3
            var graph = new Graph<Vertex, DirectedEdge>
            {
                new Vertex(0),
                new Vertex(1),
                new Vertex(2),
                new DirectedEdge(0, 1),
                new DirectedEdge(1, 2),
            };

            var expectedResults = new List<int[]>
            {
                new int[] { 0 },
                new int[] { 1 },
                new int[] { 2 },
            };

            RunTest(graph, expectedResults);
        }

        [Fact]
        public void TreeDependencyOrder_ProcessedNode_1_Tree()
        {
            // Tree graph
            // 0 -> 1 -> 2
            //      1 -> 3
            var graph = new Graph<Vertex, DirectedEdge>
            {
                new Vertex(0),
                new Vertex(1),
                new Vertex(2),
                new DirectedEdge(0, 1),
                new DirectedEdge(1, 2),
            };

            var processedNodes = new int[]
            {
                0,
            };

            var expectedResults = new List<int[]>
            {
                new int[] { 1 },
                new int[] { 2 },
            };

            RunTest(graph, expectedResults, null, processedNodes);
        }

        [Fact]
        public void TreeDependencyOrder_ProcessedNode_2_Tree()
        {
            // Tree graph
            // 0 -> 1 -> 2 -> 3
            //      1 -> 3
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
                new DirectedEdge(1, 3),
            };

            var variations = new[]
            {
                new
                {
                    Results = new List<int[]>
                    {
                        new int[] { 0, 4 },
                        new int[] { 1 },
                        new int[] { 2 },
                        new int[] { 3 },
                    },
                    ProcessedNodes = new int[] { },
                },
                new
                {
                    Results = new List<int[]>
                    {
                        new int[] { 1, 4 },
                        new int[] { 2 },
                        new int[] { 3 },
                    },
                    ProcessedNodes = new int[] { 0 },
                },
                new
                {
                    Results = new List<int[]>
                    {
                        new int[] { 2, 4 },
                        new int[] { 3 },
                    },
                    ProcessedNodes = new int[] { 0, 1 },
                },
                new
                {
                    Results = new List<int[]>
                    {
                        new int[] { 1 },
                        new int[] { 2 },
                        new int[] { 3 },
                    },
                    ProcessedNodes = new int[] { 0, 4 },
                },
                new
                {
                    Results = new List<int[]>
                    {
                        new int[] { 3 },
                    },
                    ProcessedNodes = new int[] { 0, 1, 2, 4 },
                },
            };

            foreach (var item in variations)
            {
                RunTest(graph, item.Results, null, item.ProcessedNodes);
            }
        }


        [Fact]
        public void TreeDependencyOrder_Level_Tree()
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

            var expectedResults = new List<int[]>
            {
                new int[] { 0 },
            };

            RunTest(graph, expectedResults, expectedResults.Count);

            expectedResults = new List<int[]>
            {
                new int[] { 0 },
                new int[] { 1 },
            };

            RunTest(graph, expectedResults, expectedResults.Count);

            expectedResults = new List<int[]>
            {
                new int[] { 0 },
                new int[] { 1 },
                new int[] { 2, 4 },
            };

            RunTest(graph, expectedResults, expectedResults.Count);
        }

        [Fact]
        public void TreeDependencyOrder_Right_Tree()
        {
            // Tree graph
            // 0 <- 1 <- 2 <- 3
            //      1 <- 4
            var graph = new Graph<Vertex, DirectedEdge>
            {
                new Vertex(0),
                new Vertex(1),
                new Vertex(2),
                new Vertex(3),
                new Vertex(4),
                new DirectedEdge(3, 2),
                new DirectedEdge(2, 1),
                new DirectedEdge(1, 0),
                new DirectedEdge(4, 1),
            };

            var expectedResults = new List<int[]>
            {
                new int[] { 3, 4 },
                new int[] { 2 },
                new int[] { 1 },
                new int[] { 0 },
            };

            RunTest(graph, expectedResults);
        }

        [Fact]
        public void TreeDependencyOrder_Right_1_ProcessedNodesTest()
        {
            // Tree graph
            // 0 <- 1 <- 2 <- 3
            //      1 <- 4
            var graph = new Graph<Vertex, DirectedEdge>
            {
                new Vertex(0),
                new Vertex(1),
                new Vertex(2),
                new Vertex(3),
                new Vertex(4),
                new DirectedEdge(3, 2),
                new DirectedEdge(2, 1),
                new DirectedEdge(1, 0),
                new DirectedEdge(4, 1),
            };

            var variations = new[]
            {
                new
                {
                    Results = new List<int[]>
                    {
                        new int[] { 3, 4 },
                        new int[] { 2 },
                        new int[] { 1 },
                        new int[] { 0 },
                    },
                    ProcessedNodes = new int[] { },
                },
                new
                {
                    Results = new List<int[]>
                    {
                        new int[] { 2, 4 },
                        new int[] { 1 },
                        new int[] { 0 },
                    },
                    ProcessedNodes = new int[] { 3 },
                },
                new
                {
                    Results = new List<int[]>
                    {
                        new int[] { 2 },
                        new int[] { 1 },
                        new int[] { 0 },
                    },
                    ProcessedNodes = new int[] { 3, 4 },
                },
                new
                {
                    Results = new List<int[]>
                    {
                        new int[] { 4 },
                        new int[] { 1 },
                        new int[] { 0 },
                    },
                    ProcessedNodes = new int[] { 3, 2 },
                },
                new
                {
                    Results = new List<int[]>
                    {
                        new int[] { 0, 4 },
                    },
                    ProcessedNodes = new int[] { 3, 2, 1 },
                },
            };

            foreach (var item in variations)
            {
                RunTest(graph, item.Results, null, item.ProcessedNodes);
            }
        }

        private void RunTest(Graph<Vertex, DirectedEdge> graph, List<int[]> expectedResults, int? numberOfLevels = null, int[] processedNodes = null)
        {
            var x1 = graph.GetTopologicalOrdering(numberOfLevels, processedNodes);
            x1.Count.Should().Be(expectedResults.Count);

            var x2 = x1
                .Zip(expectedResults, (o, i) => new { o = o.OrderBy(x => x), i });

            x2.All(x => x.o.SequenceEqual(x.i))
                .Should().BeTrue();
        }
    }
}
