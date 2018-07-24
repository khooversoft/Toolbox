using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Telemetry
{
    //
    // Summary:
    //     Identifies the level of an event.
    public enum TelemetryLevel
    {
        //
        // Summary:
        //     No level filtering is done on the event.
        LogAlways = 0,
        //
        // Summary:
        //     This level corresponds to a critical error, which is a serious error that has
        //     caused a major failure.
        Critical = 1,
        //
        // Summary:
        //     This level adds standard errors that signify a problem.
        Error = 2,
        //
        // Summary:
        //     This level adds warning events (for example, events that are published because
        //     a disk is nearing full capacity).
        Warning = 3,
        //
        // Summary:
        //     This level adds informational events or messages that are not errors. These events
        //     can help trace the progress or state of an application.
        Informational = 4,
        //
        // Summary:
        //     This level adds lengthy events or messages. It causes all events to be logged.
        Verbose = 5
    }
}
