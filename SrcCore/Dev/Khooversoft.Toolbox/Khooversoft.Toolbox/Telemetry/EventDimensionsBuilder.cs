using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Build event dimension dictionary
    /// </summary>
    public class EventDimensionsBuilder
    {
        private readonly List<KeyValuePair<string, object>> _properties;

        public EventDimensionsBuilder()
        {
            _properties = new List<KeyValuePair<string, object>>();
        }

        public EventDimensionsBuilder(IEnumerable<KeyValuePair<string, object>> values)
        {
            _properties = values.ToList();
        }

        public EventDimensionsBuilder Add(string key, object value)
        {
            _properties.Add(new KeyValuePair<string, object>(key, value));
            return this;
        }

        public EventDimensionsBuilder Clear()
        {
            _properties.Clear();
            return this;
        }

        public IEventDimensions Build()
        {
            return new EventDimensions(_properties);
        }
    }
}
