using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Actor
{
    [EventSource(Name = "Khooversoft.Actor")]
    public class ActorEventSource : EventSource, IEventLog
    {
        private enum EventIds
        {
            ActivityStart = 1,
            ActivityStop,
            Info,
            Error,
            Warning,
            Verbose,
            ActorActivate = 100,
            ActorDeactivate,
            ActorCalling,
            ActorCalled,
            ActorRegistered,
            ActorStartTimer,
            ActorStopTimer,
            ActorTimerCallback
        };

        readonly string _machineName;
        readonly string _processName;

        public ActorEventSource()
            : base(EventSourceSettings.ThrowOnEventWriteErrors)
        {
            _machineName = Environment.MachineName;
            _processName = Process.GetCurrentProcess().ProcessName;
        }

        public class Keywords
        {
            public const EventKeywords Diagnostic = (EventKeywords)0x1;
            public const EventKeywords Perf = (EventKeywords)0x2;
        }

        public static ActorEventSource Log { get; } = new ActorEventSource();

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void ActivityStart(IWorkContext context, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);

            ActivityStart(_machineName, _processName, context.Cv, context.Tag, message);
        }

        [Event((int)EventIds.ActivityStart, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Perf)]
        private void ActivityStart(string machineName, string processName, string cv, string tag, string message)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.ActivityStart, machineName, processName, cv, tag, message);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void ActivityStop(IWorkContext context, string message = null, long durationMs = 0)
        {
            Verify.IsNotNull(nameof(context), context);

            ActivityStop(_machineName, _processName, context.Cv, context.Tag, message, durationMs);
        }

        [Event((int)EventIds.ActivityStop, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Perf)]
        private void ActivityStop(string machineName, string processName, string cv, string tag, string message, long durationMs)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.ActivityStop, machineName, processName, cv, tag, message, durationMs);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void Info(IWorkContext context, string message)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(message), message);

            Info(_machineName, _processName, context.Cv, context.Tag, message);
        }

        [Event((int)EventIds.Info, Message = "x{4}", Level = EventLevel.Informational, Keywords = Keywords.Diagnostic)]
        private void Info(string machineName, string processName, string cv, string tag, string message)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.Info, machineName, processName, cv, tag, message);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void Error(IWorkContext context, string message, Exception exception = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(message), message);

            Error(_machineName, _processName, context.Cv, context.Tag, message, exception?.ToString());
        }

        [Event((int)EventIds.Error, Message = "x{4}", Level = EventLevel.Error, Keywords = Keywords.Diagnostic)]
        private void Error(string machineName, string processName, string cv, string tag, string message, string exception)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.Error, machineName, processName, cv, tag, message, exception);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void Warning(IWorkContext context, string message, Exception exception = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(message), message);

            Warning(_machineName, _processName, context.Cv, context.Tag, message, exception?.ToString());
        }

        [Event((int)EventIds.Warning, Message = "x{4}", Level = EventLevel.Warning, Keywords = Keywords.Diagnostic)]
        private void Warning(string machineName, string processName, string cv, string tag, string message, string exception)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.Warning, machineName, processName, cv, tag, message, exception);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void Verbose(IWorkContext context, string message, Exception exception = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(message), message);

            Verbose(_machineName, _processName, context.Cv, context.Tag, message, exception?.ToString());
        }

        [Event((int)(int)EventIds.Verbose, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Diagnostic)]
        private void Verbose(string machineName, string processName, string cv, string tag, string message, string exception)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.Verbose, machineName, processName, cv, tag, message, exception);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void ActorActivate(IWorkContext context, ActorKey actorKey, Type actorType, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(actorType), actorType);

            ActorActivate(_machineName, _processName, context.Cv, context.Tag, actorKey.ToString(), actorType.FullName, message);
        }

        [Event((int)(int)EventIds.ActorActivate, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Diagnostic)]
        private void ActorActivate(string machineName, string processName, string cv, string tag, string actorKey, string interfaceType, string message)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.ActorActivate, machineName, processName, cv, tag, actorKey, interfaceType, message);
            }
        }
        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void ActorDeactivate(IWorkContext context, ActorKey actorKey, Type actorType, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(actorType), actorType);

            ActorDeactivate(_machineName, _processName, context.Cv, context.Tag, actorKey.ToString(), actorType.FullName, message);
        }

        [Event((int)(int)EventIds.ActorDeactivate, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Diagnostic)]
        private void ActorDeactivate(string machineName, string processName, string cv, string tag, string actorKey, string interfaceType, string message)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.ActorDeactivate, machineName, processName, cv, tag, actorKey, interfaceType, message);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void ActorCalling(IWorkContext context, ActorKey actorKey, Type actorType, string methodName, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(actorType), actorType);
            Verify.IsNotEmpty(nameof(methodName), methodName);

            ActorCalling(_machineName, _processName, context.Cv, context.Tag, actorKey.ToString(), methodName, actorType.FullName, message);
        }

        [Event((int)(int)EventIds.ActorCalling, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Diagnostic)]
        private void ActorCalling(string machineName, string processName, string cv, string tag, string actorKey, string methodName, string actorType, string message)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.ActorCalling, machineName, processName, cv, tag, actorKey, methodName, actorType, message);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void ActorCalled(IWorkContext context, ActorKey actorKey, Type interfaceType, string methodName, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorKey), actorKey);
            Verify.IsNotNull(nameof(interfaceType), interfaceType);
            Verify.IsNotEmpty(nameof(methodName), methodName);

            ActorCalled(_machineName, _processName, context.Cv, context.Tag, actorKey.ToString(), methodName, interfaceType.FullName, message);
        }

        [Event((int)(int)EventIds.ActorCalled, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Diagnostic)]
        private void ActorCalled(string machineName, string processName, string cv, string tag, string actorKey, string methodName, string interfaceType, string message)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.ActorCalled, machineName, processName, cv, tag, actorKey, methodName, interfaceType, message);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void ActorRegistered(IWorkContext context, Type interfaceType, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(interfaceType), interfaceType);

            ActorRegistered(_machineName, _processName, context.Cv, context.Tag, interfaceType.FullName, message);
        }

        [Event((int)(int)EventIds.ActorRegistered, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Diagnostic)]
        private void ActorRegistered(string machineName, string processName, string cv, string tag, string interfaceType, string message)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.ActorRegistered, machineName, processName, cv, tag, interfaceType, message);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void ActorStartTimer(IWorkContext context, ActorKey actorKey, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorKey), actorKey);

            ActorStartTimer(_machineName, _processName, context.Cv, context.Tag, actorKey.ToString(), message);
        }

        [Event((int)(int)EventIds.ActorStartTimer, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Diagnostic)]
        private void ActorStartTimer(string machineName, string processName, string cv, string tag, string actorId, string message)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.ActorStartTimer, machineName, processName, cv, tag, actorId, message);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void ActorStopTimer(IWorkContext context, ActorKey actorKey, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorKey), actorKey);

            ActorStopTimer(_machineName, _processName, context.Cv, context.Tag, actorKey.ToString(), message);
        }

        [Event((int)(int)EventIds.ActorStopTimer, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Diagnostic)]
        private void ActorStopTimer(string machineName, string processName, string cv, string tag, string actorId, string message)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.ActorStopTimer, machineName, processName, cv, tag, actorId, message);
            }
        }

        // ========================================================================================
        // ========================================================================================

        [NonEvent]
        public void ActorTimerCallback(IWorkContext context, ActorKey actorKey, string message = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(actorKey), actorKey);

            ActorTimerCallback(_machineName, _processName, context.Cv, context.Tag, actorKey.ToString(), message);
        }

        [Event((int)(int)EventIds.ActorTimerCallback, Message = "x{4}", Level = EventLevel.Verbose, Keywords = Keywords.Diagnostic)]
        private void ActorTimerCallback(string machineName, string processName, string cv, string tag, string actorId, string message)
        {
            if (IsEnabled())
            {
                WriteEvent((int)EventIds.ActorTimerCallback, machineName, processName, cv, tag, actorId, message);
            }
        }
    }
}
