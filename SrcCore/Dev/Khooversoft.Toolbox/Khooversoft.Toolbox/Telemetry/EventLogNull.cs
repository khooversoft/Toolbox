using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox
{
    public class EventLogNull : IEventLog
    {
        public EventLogNull()
        {
        }

        public void ActivityStart(IWorkContext context, string message = null, IEventDimensions dimensions = null)
        {
        }

        public void ActivityStart(IWorkContext context, string message, object dimensions)
        {
        }

        public void ActivityStop(IWorkContext context, string message = null, long durationMs = 0, IEventDimensions dimensions = null)
        {
        }

        public void ActivityStop(IWorkContext context, string message, long durationMs, object dimensions)
        {
        }

        public void Critial(IWorkContext context, string message, Exception exception = null, IEventDimensions dimensions = null)
        {
        }

        public void Critial(IWorkContext context, string message, object dimensions, Exception exception = null)
        {
        }

        public void Error(IWorkContext context, string message, Exception exception = null, IEventDimensions dimensions = null)
        {
        }

        public void Error(IWorkContext context, string message, object dimensions, Exception exception = null)
        {
        }

        public void Info(IWorkContext context, string message, IEventDimensions dimensions = null)
        {
        }

        public void Info(IWorkContext context, string message, object dimensions)
        {
        }

        public void TrackMetric(IWorkContext context, string name, IEventDimensions dimensions = null)
        {
        }

        public void TrackMetric(IWorkContext context, string name, object dimensions)
        {
        }

        public void TrackMetric(IWorkContext context, string name, double value, IEventDimensions dimensions = null)
        {
        }

        public void TrackMetric(IWorkContext context, string name, double value, object dimensions)
        {
        }

        public void Verbose(IWorkContext context, string message, IEventDimensions dimensions = null)
        {
        }

        public void Verbose(IWorkContext context, string message, object dimensions)
        {
        }
    }
}
