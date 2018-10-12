using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;

namespace Khooversoft.Telemetry
{
    public class TrackEventSource : IEventLog
    {
        private const string _messageName = "message";
        private readonly EventRouter _router;
        private readonly string _eventSourceName;

        public TrackEventSource(EventRouter router, string eventSourceName)
        {
            Verify.IsNotNull(nameof(router), router);
            Verify.IsNotEmpty(nameof(eventSourceName), eventSourceName);

            _router = router;
            _eventSourceName = eventSourceName;
        }

        public void ActivityStart(IWorkContext context, string message = null, IEventDimensions dimensions = null)
        {
            Verify.IsNotNull(nameof(context), context);

            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(ActivityStart), TelemetryType.Verbose, context.Cv, context.Tag, dim));
        }

        public void ActivityStart(IWorkContext context, string message, object dimensions)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(ActivityStart), TelemetryType.Verbose, context.Cv, context.Tag, dim));
        }

        public void ActivityStop(IWorkContext context, string message = null, long durationMs = 0, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(ActivityStop), TelemetryType.Verbose, context.Cv, context.Tag, dim));
        }

        public void ActivityStop(IWorkContext context, string message, long durationMs, object dimensions)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(ActivityStop), TelemetryType.Verbose, context.Cv, context.Tag, dim));
        }

        public void Critial(IWorkContext context, string message, Exception exception = null, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(Critial), TelemetryType.Critical, context.Cv, context.Tag, dim));
        }

        public void Critial(IWorkContext context, string message, object dimensions, Exception exception = null)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(Critial), TelemetryType.Critical, context.Cv, context.Tag, dim));
        }

        public void Error(IWorkContext context, string message, Exception exception = null, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(Error), TelemetryType.Error, context.Cv, context.Tag, dim));
        }

        public void Error(IWorkContext context, string message, object dimensions, Exception exception = null)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(Error), TelemetryType.Error, context.Cv, context.Tag, dim));
        }

        public void Info(IWorkContext context, string message, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(Info), TelemetryType.Informational, context.Cv, context.Tag, dim));
        }

        public void Info(IWorkContext context, string message, object dimensions)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(Info), TelemetryType.Informational, context.Cv, context.Tag, dim));
        }

        public void Verbose(IWorkContext context, string message, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(Verbose), TelemetryType.Verbose, context.Cv, context.Tag, dim));
        }

        public void Verbose(IWorkContext context, string message, object dimensions)
        {
            var dim = context.Materialized(message, dimensions);
            _router.Post(new EventData(_eventSourceName, nameof(Verbose), TelemetryType.Verbose, context.Cv, context.Tag, dim));
        }

        // ========================================================================================
        // Log event
        public void LogEvent(IWorkContext context, TelemetryType telemetryLevel, string eventSourceName, string eventName, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(eventSourceName, eventName, telemetryLevel, context.Cv, context.Tag, dim));
        }

        public void LogEvent(IWorkContext context, TelemetryType telemetryLevel, string eventSourceName, string eventName, object dimensions)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(eventSourceName, eventName, telemetryLevel, context.Cv, context.Tag, dim));
        }

        // ========================================================================================
        // Metrics
        public void TrackMetric(IWorkContext context, string name, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(_eventSourceName, name, TelemetryType.Metric, context.Cv, context.Tag, dim));
        }

        public void TrackMetric(IWorkContext context, string name, object dimensions)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(_eventSourceName, name, TelemetryType.Metric, context.Cv, context.Tag, dim));
        }

        public void TrackMetric(IWorkContext context, string name, double value, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(_eventSourceName, name, TelemetryType.Metric, context.Cv, context.Tag, value, dim));
        }

        public void TrackMetric(IWorkContext context, string name, double value, object dimensions)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(_eventSourceName, name, TelemetryType.Metric, context.Cv, context.Tag, value, dim));
        }
    }
}
