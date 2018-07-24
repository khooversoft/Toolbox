using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Telemetry
{
    public static class TelemetryExtensions
    {
        public static void Verbose(this ITelemetry self, IWorkContext context, Func<string> getMessage, IEnumerable<string> dimensions = null)
        {
            Verify.IsNotNull(nameof(self), self);

            self.Log(context, TelemetryLevel.Verbose, getMessage, dimensions);
        }

        public static void Info(this ITelemetry self, IWorkContext context, Func<string> getMessage, IEnumerable<string> dimensions = null)
        {
            Verify.IsNotNull(nameof(self), self);

            self.Log(context, TelemetryLevel.Informational, getMessage, dimensions);
        }

        public static void Warning(this ITelemetry self, IWorkContext context, Func<string> getMessage, IEnumerable<string> dimensions = null, Exception exception = null)
        {
            Verify.IsNotNull(nameof(self), self);

            self.Log(context, TelemetryLevel.Warning, getMessage, dimensions, exception);
        }

        public static void Error(this ITelemetry self, IWorkContext context, Func<string> getMessage, IEnumerable<string> dimensions = null, Exception exception = null)
        {
            Verify.IsNotNull(nameof(self), self);

            self.Log(context, TelemetryLevel.Error, getMessage, dimensions, exception);
        }

        public static void Critial(this ITelemetry self, IWorkContext context, Func<string> getMessage, IEnumerable<string> dimensions = null, Exception exception = null)
        {
            Verify.IsNotNull(nameof(self), self);

            self.Log(context, TelemetryLevel.Critical, getMessage, dimensions, exception);
        }
    }
}
