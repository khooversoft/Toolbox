using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public interface IEventLog
    {
        void ActivityStart(IWorkContext context, string message = null);
        void ActivityStop(IWorkContext context, string message = null, long durationMs = 0);
        void Info(IWorkContext context, string message);
        void Error(IWorkContext context, string message, Exception exception = null);
        void Verbose(IWorkContext context, string message, Exception exception = null);
    }
}
