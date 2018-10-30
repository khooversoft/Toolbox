using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;

namespace Khooversoft.Telemetry
{
    public class TrackEventSource : ITelemetry
    {
        private const string _messageName = "message";
        private readonly EventRouter _router;

        public TrackEventSource(EventRouter router, string eventSourceName)
        {
            Verify.IsNotNull(nameof(router), router);
            Verify.IsNotEmpty(nameof(eventSourceName), eventSourceName);

            _router = router;
            EventSourceName = eventSourceName;
        }

        public string EventSourceName { get; }

        public void ActivityStart(IWorkContext context, string message = null, IEventDimensions dimensions = null)
        {
            Verify.IsNotNull(nameof(context), context);

            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(ActivityStart), TelemetryLevel.Verbose, message, context.Cv, context.Tag, dim));
        }

        public void ActivityStart(IWorkContext context, string message, object dimensions)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(ActivityStart), TelemetryLevel.Verbose, message, context.Cv, context.Tag, dim));
        }

        public void ActivityStop(IWorkContext context, string message = null, long durationMs = 0, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(ActivityStop), TelemetryLevel.Verbose, message, context.Cv, context.Tag, dim));
        }

        public void ActivityStop(IWorkContext context, string message, long durationMs, object dimensions)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(ActivityStop), TelemetryLevel.Verbose, message, context.Cv, context.Tag, dim));
        }

        public void Critical(IWorkContext context, string message, Exception exception = null, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(Critical), TelemetryLevel.Critical, message, context.Cv, context.Tag, dim));
        }

        public void Critical(IWorkContext context, string message, object dimensions, Exception exception = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(Critical), TelemetryLevel.Critical, message, context.Cv, context.Tag, dim));
        }

        public void Error(IWorkContext context, string message, Exception exception = null, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(Error), TelemetryLevel.Error, message, context.Cv, context.Tag, dim));
        }

        public void Error(IWorkContext context, string message, object dimensions, Exception exception = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(Error), TelemetryLevel.Error, message, context.Cv, context.Tag, dim));
        }

        public void Info(IWorkContext context, string message, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(Info), TelemetryLevel.Informational, message, context.Cv, context.Tag, dim));
        }

        public void Info(IWorkContext context, string message, object dimensions)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(Info), TelemetryLevel.Informational, message, context.Cv, context.Tag, dim));
        }

        public void Verbose(IWorkContext context, string message, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(Verbose), TelemetryLevel.Verbose, message, context.Cv, context.Tag, dim));
        }

        public void Verbose(IWorkContext context, string message, object dimensions)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, nameof(Verbose), TelemetryLevel.Verbose, message, context.Cv, context.Tag, dim));
        }

        // ========================================================================================
        // Log event
        public void LogEvent(IWorkContext context, TelemetryLevel telemetryLevel, string eventSourceName, string eventName, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(eventSourceName, eventName, telemetryLevel, context.Cv, context.Tag, dim));
        }

        public void LogEvent(IWorkContext context, TelemetryLevel telemetryLevel, string eventSourceName, string eventName, object dimensions)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(eventSourceName, eventName, telemetryLevel, context.Cv, context.Tag, dim));
        }

        // ========================================================================================
        // Metrics
        public void TrackMetric(IWorkContext context, string name, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, name, TelemetryLevel.Metric, context.Cv, context.Tag, dim));
        }

        public void TrackMetric(IWorkContext context, string name, object dimensions)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, name, TelemetryLevel.Metric, context.Cv, context.Tag, dim));
        }

        public void TrackMetric(IWorkContext context, string name, double value, IEventDimensions dimensions = null)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, name, TelemetryLevel.Metric, context.Cv, context.Tag, value, dim));
        }

        public void TrackMetric(IWorkContext context, string name, double value, object dimensions)
        {
            var dim = context.Materialized(dimensions);
            _router.Post(new EventData(EventSourceName, name, TelemetryLevel.Metric, context.Cv, context.Tag, value, dim));
        }
    }
}
