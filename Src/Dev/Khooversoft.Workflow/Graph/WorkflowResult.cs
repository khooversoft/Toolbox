using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Workflow
{
    /// <summary>
    /// Details the result of running the workflow vertex
    /// </summary>
    public class WorkflowResult
    {
        /// <summary>
        /// Workflow was executed and was successful.  State must equal
        /// 'Completed' for this to have value.
        /// </summary>
        public bool IsSuccessful { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}
