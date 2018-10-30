// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public interface ITelemetry
    {
        string EventSourceName { get; }

        void ActivityStart(IWorkContext context, string message = null, IEventDimensions dimensions = null);
        void ActivityStart(IWorkContext context, string message, object dimensions);

        void ActivityStop(IWorkContext context, string message = null, long durationMs = 0, IEventDimensions dimensions = null);
        void ActivityStop(IWorkContext context, string message, long durationMs, object dimensions);

        void Verbose(IWorkContext context, string message, IEventDimensions dimensions = null);
        void Verbose(IWorkContext context, string message, object dimensions);

        void Info(IWorkContext context, string message, IEventDimensions dimensions = null);
        void Info(IWorkContext context, string message, object dimensions);

        void Error(IWorkContext context, string message, Exception exception = null, IEventDimensions dimensions = null);
        void Error(IWorkContext context, string message, object dimensions, Exception exception = null);

        void Critical(IWorkContext context, string message, Exception exception = null, IEventDimensions dimensions = null);
        void Critical(IWorkContext context, string message, object dimensions, Exception exception = null);

        void TrackMetric(IWorkContext context, string name, IEventDimensions dimensions = null);
        void TrackMetric(IWorkContext context, string name, object dimensions);

        void TrackMetric(IWorkContext context, string name, double value, IEventDimensions dimensions = null);
        void TrackMetric(IWorkContext context, string name, double value, object dimensions);

        void LogEvent(IWorkContext context, TelemetryLevel telemetryLevel, string eventSourceName, string eventName, IEventDimensions dimensions = null);
        void LogEvent(IWorkContext context, TelemetryLevel telemetryLevel, string eventSourceName, string eventName, object dimensions);
    }
}
