using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Workflow.Test.GraphWorkflow
{
    [Trait("Category", "Toolbox")]
    public class StateWorkflowTests
    {
        private static IWorkContext _context = WorkContext.Empty;

        [Fact]
        public void NoWorkTest()
        {
            var g1 = new WorkflowGraph();
            g1.Run(_context);
        }

        [Fact]
        public void OneWorkTest()
        {
            var g1 = new WorkflowGraph()
            {
                new Work(0, new IStateItem[] { new FakeStateItem() }),
            };
            g1.Run(_context);

            g1.OfType<Work>()
                .All(x => x.StateItems.OfType<FakeStateItem>().First().IsDone)
                .Should().BeTrue();
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
            public FakeStateItem()
            {
            }

            public bool IsDone { get; private set; }

            public string Name => "Name";
            public bool IgnoreError => false;

            public Task<bool> Set(IWorkContext context)
            {
                IsDone = true;
                return Task.FromResult(true);
            }

            public Task<bool> Test(IWorkContext context)
            {
                return Task.FromResult(true);
            }
        }
    }
}
