using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Workflow
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
        public Task Run(IWorkContext context)
        {
            context = context.WithTag(_tag);

            ResetGraph();

            var tasks = new Dictionary<int, TaskReference>();
            var nodes = GetTopologicalOrdering(1).SelectMany(x => x);
            var queue = new Queue<int>(nodes);

            while (queue.Count > 0)
            {
                ProcessQueue(context, queue, tasks);

                var processedEdges = Vertices.Values
                    .Where(x => x.State != WorkflowState.None)
                    .Join(Edges.Values, x => x.NodeId, x => x.EdgeId.SourceNodeId, (o, i) => i);

                var processedToNodeIds = new HashSet<int>(processedEdges.Select(x => x.EdgeId.ToNodeId));

                var freeNode = Vertices.Values
                    .Where(x => x.State != WorkflowState.None && !processedToNodeIds.Contains(x.NodeId))
                    .Select(x => x.NodeId);

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
                WorkflowEventSource.Log.Verbose(context, $"Starting state manager for nodeId={nodeId}");

                var manager = new StateManagerBuilder()
                    .Add(vertex.StateItems)
                    .Build();

                Task t = Task.Run(() => manager.RunAsync(context));
                vertex.State = WorkflowState.Running;
                taskReferences.Add(nodeId, new TaskReference(t, nodeId));
            }

            if (taskReferences.Count > 0)
            {
                Task.WaitAny(taskReferences.Values.Select(x => x.Task).ToArray(), context.CancellationToken);
            }

            taskReferences.Values
                .Where(x => x.Task.IsCompleted)
                .Do(x => Vertices[x.NodeId].State = WorkflowState.Completed)
                .ToList()
                .Do(x => WorkflowEventSource.Log.Verbose(context, $"NodeId={x.NodeId} state manager has been completed"))
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
