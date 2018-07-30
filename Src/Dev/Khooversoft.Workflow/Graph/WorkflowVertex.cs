using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Workflow
{
    public class WorkflowVertex : Vertex
    {
        public WorkflowVertex(int nodeId, IEnumerable<IStateItem> stateItems)
            : base(nodeId)
        {
            Verify.IsNotNull(nameof(stateItems), stateItems);

            StateItems = stateItems.ToList();
        }

        public IReadOnlyList<IStateItem> StateItems { get; }

        public WorkflowState State { get; set; }

        public WorkflowResult Result { get; set; }

        public void Reset()
        {
            State = WorkflowState.None;
            Result = null;
        }
    }
}
