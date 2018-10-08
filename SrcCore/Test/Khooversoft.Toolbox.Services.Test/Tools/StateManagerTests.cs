using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Tools
{
    public class StateManagerTests
    {
        private static IWorkContext _workContext = WorkContext.Empty;

        [Fact]
        public async Task SimpleStateFlowTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemSuccess())
                .Build();

            StateContext stateContext = await workPlan.RunAsync(_workContext);
            stateContext.Should().NotBeNull();

            const int count = 1;
            stateContext.WorkItemIndex.Should().Be(count);
            workPlan.IsRunning.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(count);
            workPlan.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public async Task SimpleNotifyTest()
        {
            bool pass = false;

            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemSuccess())
                .Set(o => o.If(x => x.ActionType == StateNotify.ActionTypes.Set, x => pass = true, x => pass = false))
                .Build();

            StateContext stateContext = await workPlan.RunAsync(_workContext);
            stateContext.Should().NotBeNull();

            const int count = 1;
            stateContext.WorkItemIndex.Should().Be(count);
            pass.Should().BeTrue();
        }

        [Fact]
        public async Task MultipleStateFlowTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemSuccess())
                .Add(new StateItemSuccess())
                .Add(new StateItemSuccess())
                .Build();

            StateContext stateContext = await workPlan.RunAsync(_workContext);
            stateContext.Should().NotBeNull();

            const int count = 3;
            stateContext.WorkItemIndex.Should().Be(count);
            workPlan.IsRunning.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(count);
            workPlan.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public async Task FailureStateFlowTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemFailure())
                .Build();

            StateContext stateContext = await workPlan.RunAsync(_workContext);
            stateContext.Should().NotBeNull();

            const int count = 0;
            stateContext.WorkItemIndex.Should().Be(count);
            workPlan.IsRunning.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(1);
            workPlan.IsSuccessful.Should().BeFalse();
        }

        [Fact]
        public async Task Failure2StateFlowTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemFailure())
                .Add(new StateItemSuccess())
                .Build();

            StateContext stateContext = await workPlan.RunAsync(_workContext);
            stateContext.Should().NotBeNull();

            const int count = 0;
            stateContext.WorkItemIndex.Should().Be(count);
            workPlan.IsRunning.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(2);
            workPlan.IsSuccessful.Should().BeFalse();
        }

        [Fact]
        public async Task SuccessAndFailureStateFlowTest()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemSuccess())
                .Add(new StateItemFailure())
                .Build();

            StateContext stateContext = await workPlan.RunAsync(_workContext);
            stateContext.Should().NotBeNull();

            const int count = 1;
            stateContext.WorkItemIndex.Should().Be(count);
            workPlan.IsRunning.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(2);
            workPlan.IsSuccessful.Should().BeFalse();
        }

        [Fact]
        public async Task SuccessTestStateFlow()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemAlreadyPresent())
                .Build();

            StateContext stateContext = await workPlan.TestAsync(_workContext);
            stateContext.Should().NotBeNull();

            const int count = 1;
            stateContext.WorkItemIndex.Should().Be(count);
            workPlan.IsRunning.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(count);
            workPlan.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public async Task SuccessTest2StateStateFlow()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemAlreadyPresent())
                .Add(new StateItemAlreadyPresent())
                .Build();

            StateContext stateContext = await workPlan.TestAsync(_workContext);
            stateContext.Should().NotBeNull();

            const int count = 2;
            stateContext.WorkItemIndex.Should().Be(count);
            workPlan.IsRunning.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(count);
            workPlan.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public async Task SuccessTestFailedStateFlow()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemFailure())
                .Build();

            StateContext stateContext = await workPlan.TestAsync(_workContext);
            stateContext.Should().NotBeNull();

            const int count = 1;
            stateContext.WorkItemIndex.Should().Be(0);
            workPlan.IsRunning.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(count);
            workPlan.IsSuccessful.Should().BeFalse();
        }

        [Fact]
        public async Task SuccessTestFailed2StateStateFlow()
        {
            IStateManager workPlan = new StateManagerBuilder()
                .Add(new StateItemAlreadyPresent())
                .Add(new StateItemFailure())
                .Build();

            StateContext stateContext = await workPlan.TestAsync(_workContext);
            stateContext.Should().NotBeNull();

            const int count = 2;
            stateContext.WorkItemIndex.Should().Be(1);
            workPlan.IsRunning.Should().BeFalse();
            workPlan.StateItems.Count.Should().Be(count);
            workPlan.IsSuccessful.Should().BeFalse();
        }

        private class StateItemSuccess : StateItemBase
        {
            public StateItemSuccess()
                : base("Success", false, false, true)
            {
            }
        }

        private class StateItemFailure : StateItemBase
        {
            public StateItemFailure()
                : base("Failure", false, false, false)
            {
            }
        }

        private class StateItemAlreadyPresent : StateItemBase
        {
            public StateItemAlreadyPresent()
                : base("Present", false, true, false)
            {
            }
        }

        private class StateItemBase : IStateItem
        {
            private bool _resultFromTest;
            private bool _resultFromSet;

            public StateItemBase(string name, bool ignoreError, bool resultFromTest, bool resultFromSet)
            {
                Name = name;
                IgnoreError = ignoreError;
                _resultFromTest = resultFromTest;
                _resultFromSet = resultFromSet;
            }

            public string Name { get; }

            public bool IgnoreError { get; }

            public Task<bool> Set(IWorkContext context)
            {
                return Task.FromResult(_resultFromSet);
            }

            public Task<bool> Test(IWorkContext context)
            {
                return Task.FromResult(_resultFromTest);
            }
        }
    }
}
