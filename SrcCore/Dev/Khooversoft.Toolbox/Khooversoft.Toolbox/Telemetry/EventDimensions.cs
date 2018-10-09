using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox
{
    public class EventDimensions : List<KeyValuePair<string, object>>, IEventDimensions
    {
        public EventDimensions()
        {
        }

        public EventDimensions(IEnumerable<KeyValuePair<string, object>> values)
            : base(values)
        {
        }

        public static IEventDimensions Empty { get; } = new EventDimensions();
    }
}
