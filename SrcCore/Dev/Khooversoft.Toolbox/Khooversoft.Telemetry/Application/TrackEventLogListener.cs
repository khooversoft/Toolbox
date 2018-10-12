using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Telemetry
{
    public class TrackEventLogListener
    {
        private ILogFileWriter _logWriter;

        public TrackEventLogListener(ILogFileWriter logWriter)
        {
            Verify.IsNotNull(nameof(logWriter), logWriter);

            _logWriter = logWriter;
        }

        public void Post(EventData eventData)
        {
            _logWriter.Write(eventData);
        }
    }
}
