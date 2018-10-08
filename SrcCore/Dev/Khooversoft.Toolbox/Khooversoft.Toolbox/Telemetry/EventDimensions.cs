using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox
{
    public class EventDimensions : List<KeyValuePair<string, string>>, IEventDimensions
    {
        public EventDimensions()
        {
        }

        public EventDimensions(IEnumerable<KeyValuePair<string, string>> values)
            : base(values)
        {
        }

        public static IEventDimensions Empty { get; } = new EventDimensions();
    }
}
