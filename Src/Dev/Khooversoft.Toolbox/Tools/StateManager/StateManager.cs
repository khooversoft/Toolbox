using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public class StateManager : IStateManager
    {
        private enum RunStatus
        {
            Stopped,
            Running,
        };

        private Tag _tag = new Tag(nameof(StateManager));
        private int _running = (int)RunStatus.Stopped;
        private int _isSuccessful;

        public StateManager(StateManagerBuilder builder)
        {
            Verify.IsNotNull(nameof(builder), builder);

            StateItems = new List<IStateItem>(builder.StateItems);
            Notify = builder.Notify;
        }

        public IReadOnlyList<IStateItem> StateItems { get; }

        public Action<StateNotify> Notify { get; }

        public bool IsRunning { get { return _running == (int)RunStatus.Running; } }

        public bool IsSuccessful { get { return _isSuccessful == 1; } }

        /// <summary>
        /// Run in a task
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="token">token</param>
        /// <returns>State context (result)</returns>
        public StateContext Run(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            return Task.Run(() => RunAsync(context))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Run the work item(s) in the plan
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="token">cancellation token</param>
        /// <returns>State context (result)</returns>
        public async Task<StateContext> RunAsync(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            var stateContext = new StateContext();

            context = context
                .ToBuilder()
                .Set(stateContext)
                .Build();

            int isRunning = Interlocked.CompareExchange(ref _running, (int)RunStatus.Running, (int)RunStatus.Stopped);
            if (isRunning == (int)RunStatus.Running)
            {
                throw new InvalidOperationException("State plan is already running");
            }

            try
            {
                Interlocked.Exchange(ref _isSuccessful, 0);
                ToolboxEventSource.Log.Verbose(context, "Running state plan");

                stateContext.WorkItemIndex = 0;

                foreach (var item in StateItems)
                {
                    bool result = await item.Test(context);
                    Notify?.Invoke(new StateNotify(item, StateNotify.ActionTypes.Test, result));

                    context.CancellationToken.ThrowIfCancellationRequested();
                    ToolboxEventSource.Log.Verbose(context, $"Executed state plan 'Test' for item {item.Name} with {result} result");

                    if (!result)
                    {
                        result = await item.Set(context);
                        Notify?.Invoke(new StateNotify(item, StateNotify.ActionTypes.Set, result));

                        context.CancellationToken.ThrowIfCancellationRequested();
                        ToolboxEventSource.Log.Verbose(context, $"Executed state plan 'Set' for item {item.Name} with {result} result");

                        if (!result && !item.IgnoreError)
                        {
                            ToolboxEventSource.Log.Verbose(context, $"State plan did not completed successfully, workItemIndex={stateContext.WorkItemIndex}");
                            return stateContext;
                        }
                    }

                    stateContext.WorkItemIndex++;
                }

                Interlocked.Exchange(ref _isSuccessful, 1);
                stateContext.IsSuccessFul = true;

                ToolboxEventSource.Log.Verbose(context, "State plan completed successfully");
                return stateContext;
            }
            finally
            {
                Interlocked.Exchange(ref _running, (int)RunStatus.Stopped);
                ToolboxEventSource.Log.Verbose(context, "State plan exited");
            }
        }

        public StateContext Test(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            return Task.Run(() => TestAsync(context))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public async Task<StateContext> TestAsync(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            var stateContext = new StateContext();

            context = context
                .ToBuilder()
                .Set(stateContext)
                .Build();

            int isRunning = Interlocked.CompareExchange(ref _running, (int)RunStatus.Running, (int)RunStatus.Stopped);
            if (isRunning == (int)RunStatus.Running)
            {
                throw new InvalidOperationException("State plan is already running");
            }

            try
            {
                Interlocked.Exchange(ref _isSuccessful, 0);
                ToolboxEventSource.Log.Verbose(context, "Running state plan");

                stateContext.WorkItemIndex = 0;

                foreach (var item in StateItems)
                {
                    bool result = await item.Test(context);
                    Notify?.Invoke(new StateNotify(item, StateNotify.ActionTypes.Test, result));

                    context.CancellationToken.ThrowIfCancellationRequested();
                    ToolboxEventSource.Log.Verbose(context, $"Executed state plan 'Test' for item {item.Name} with {result} result");

                    if (!result)
                    {
                        return stateContext;
                    }

                    stateContext.WorkItemIndex++;
                }

                Interlocked.Exchange(ref _isSuccessful, 1);
                stateContext.IsSuccessFul = true;

                ToolboxEventSource.Log.Verbose(context, "State plan completed successfully");
                return stateContext;
            }
            finally
            {
                Interlocked.Exchange(ref _running, (int)RunStatus.Stopped);
                ToolboxEventSource.Log.Verbose(context, "State plan exited");
            }
        }

        public StateManagerBuilder ToBuilder()
        {
            return new StateManagerBuilder(this);
        }
    }
}
