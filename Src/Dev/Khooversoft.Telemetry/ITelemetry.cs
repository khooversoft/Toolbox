using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Telemetry
{
    public interface ITelemetry
    {
        void ActivityStart(IWorkContext context, Func<string> getMessage, IEnumerable<string> dimensions = null);

        void ActivityStop(IWorkContext context, Func<string> getMessage, long durationMs = 0, IEnumerable<string> dimensions = null);

        void Log(IWorkContext context, TelemetryLevel level, Func<string> getMessage, IEnumerable<string> dimensions = null, Exception exception = null);

        void Info(IWorkContext context, Func<string> getMessage, IEnumerable<string> dimensions = null);

        void Error(IWorkContext context, Func<string> getMessage, IEnumerable<string> dimensions = null, Exception exception = null);

        void Verbose(IWorkContext context, Func<string> getMessage, IEnumerable<string> dimensions = null, Exception exception = null);

        void Metric(IWorkContext context, long value, IEnumerable<string> dimensions);
    }
}
