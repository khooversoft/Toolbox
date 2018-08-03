using Khooversoft.Toolbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public class WorkflowGraph : Graph<WorkflowVertex, DirectedEdge>
    {
        private static readonly Tag _tag = new Tag(nameof(WorkflowGraph));

        public WorkflowGraph(int maxParallel = 100)
        {
            MaxParallel = maxParallel;
        }

        public int MaxParallel { get; }

        public void ResetGraph()
        {
            this.OfType<WorkflowVertex>().Do(x => x.Reset());
        }

        /// <summary>
        /// Process workflow graph based on dependencies
        /// </summary>
        /// <returns></returns>
        public Task RunParallel(IWorkContext context)
        {
            context = context.WithTag(_tag);

            ResetGraph();

            var tasks = new Dictionary<int, TaskReference>();
            var nodes = GetTopologicalOrdering(1).SelectMany(x => x);
            var queue = new Queue<int>(nodes);

            while (queue.Count > 0)
            {
                ProcessQueue(context, queue, tasks);

                var processedNodeIds = Vertices.Values
                    .Where(x => x.State == WorkflowState.Completed)
                    .Select(x => x.NodeId);

                var inflightNodeIds = Vertices.Values
                    .Where(x => x.State == WorkflowState.Running)
                    .Select(x => x.NodeId)
                    .ToHashSet();

                var freeNode = GetTopologicalOrdering(1, processedNodeIds)
                    .SelectMany(x => x)
                    .Where(x => !inflightNodeIds.Contains(x));

                if (!freeNode.Any())
                {
                    break;
                }

                freeNode.Run(x => queue.Enqueue(x));
            }

            Task.WaitAll(tasks.Values.Select(x => x.Task).ToArray(), context.CancellationToken);
            return Task.FromResult(0);
        }

        private void ProcessQueue(IWorkContext context, Queue<int> queue, IDictionary<int, TaskReference> taskReferences)
        {
            context = context.WithTag(_tag);

            while (queue.Count > 0 && taskReferences.Count < MaxParallel)
            {
                int nodeId = queue.Dequeue();
                WorkflowVertex vertex = Vertices[nodeId];
                ToolboxEventSource.Log.Verbose(context, $"Starting state manager for nodeId={nodeId}");

                var manager = new StateManagerBuilder()
                    .Add(vertex.StateItems)
                    .Build();

                vertex.State = WorkflowState.Running;
                Task t = Task.Run(() => manager.RunAsync(context));
                taskReferences.Add(nodeId, new TaskReference(t, nodeId));
            }

            if (taskReferences.Count > 0)
            {
                Task[] waitTasks = taskReferences.Values.Select(x => x.Task).ToArray();
                int index = Task.WaitAny(waitTasks, context.CancellationToken);
                waitTasks[index].Wait();
            }

            taskReferences.Values
                .Where(x => x.Task.IsCompleted)
                .Do(x => Vertices[x.NodeId].State = WorkflowState.Completed)
                .ToList()
                .Do(x => ToolboxEventSource.Log.Verbose(context, $"NodeId={x.NodeId} state manager has been completed"))
                .Run(x => taskReferences.Remove(x.NodeId));
        }

        private struct TaskReference
        {
            public TaskReference(Task task, int nodeId)
            {
                Task = task;
                NodeId = nodeId;
            }

            public Task Task { get; }
            public int NodeId { get; }
        }
    }
}
