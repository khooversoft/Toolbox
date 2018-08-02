using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Workflow.Test.GraphWorkflow
{
    [Trait("Category", "Toolbox")]
    public class StateWorkflowTests
    {
        private static IWorkContext _context = WorkContext.Empty;
        private static int _testState;
        private static ConcurrentQueue<int> _traceQueue;

        [Fact]
        public void NoWorkTest()
        {
            var graph = new WorkflowGraph();
            graph.RunParallel(_context);
        }

        [Fact]
        public void OneWorkTest()
        {
            _testState = 0;

            var graph = new WorkflowGraph()
            {
                new Work(0, new IStateItem[] { new FakeStateItem(0, 0) }),
            };

            graph.RunParallel(_context);

            graph.OfType<Work>()
                .All(x => x.StateItems.OfType<FakeStateItem>().First().IsDone)
                .Should().BeTrue();
        }

        [Fact]
        public void TwoDependencyTest()
        {
            _testState = -1;
            _traceQueue = new ConcurrentQueue<int>();

            var graph = new WorkflowGraph()
            {
                new Work(0, new IStateItem[] { new FakeStateItem(-1, 0) }),
                new Work(1, new IStateItem[] { new FakeStateItem(0, 1) }),
                new DirectedEdge(0, 1),
            };

            graph.RunParallel(_context);

            graph.OfType<Work>()
                .All(x => x.StateItems.OfType<FakeStateItem>().First().IsDone)
                .Should().BeTrue();

            _testState.Should().Be(1);
        }

        [Fact]
        public void ThreeDependencyTest()
        {
            _testState = -1;
            _traceQueue = new ConcurrentQueue<int>();

            var graph = new WorkflowGraph()
            {
                new Work(0, new IStateItem[] { new FakeStateItem(-1, 0) }),
                new Work(1, new IStateItem[] { new FakeStateItem(0, 1) }),
                new Work(2, new IStateItem[] { new FakeStateItem(1, 2) }),
                new DirectedEdge(0, 1),
                new DirectedEdge(1, 2),
            };

            graph.RunParallel(_context);

            graph.OfType<Work>()
                .All(x => x.StateItems.OfType<FakeStateItem>().First().IsDone)
                .Should().BeTrue();

            _testState.Should().Be(2);
        }

        private class Work : WorkflowVertex
        {
            public Work(int nodeId, IEnumerable<IStateItem> stateItems)
                : base(nodeId, stateItems)
            {
            }

            public bool IsSuccess { get; set; }
        }

        private class FakeStateItem : IStateItem
        {
            public FakeStateItem(int fromState, int toState)
            {
                FromState = fromState;
                ToState = toState;
            }

            public bool IsDone { get; private set; }

            public string Name => "Name";
            public bool IgnoreError => false;

            public int FromState { get; }
            public int ToState { get; }

            public Task<bool> Set(IWorkContext context)
            {
                int expectedFromState = Interlocked.CompareExchange(ref _testState, ToState, FromState);
                expectedFromState.Should().Be(FromState);
                _traceQueue.Enqueue(ToState);

                IsDone = true;
                return Task.FromResult(true);
            }

            public Task<bool> Test(IWorkContext context)
            {
                return Task.FromResult(false);
            }
        }
    }
}
