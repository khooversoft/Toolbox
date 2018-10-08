using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox
{
    public class EventDimensionsBuilder
    {
        private const string _idName = "id";
        private const string _cvName = "cv";
        private const string _activityIdName = "activityId";
        private const string _parentActivityIdName = "parentActivityId";

        private readonly Dictionary<string, string> _properties;

        public EventDimensionsBuilder()
        {
            _properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public EventDimensionsBuilder(IReadOnlyDictionary<string, string> values)
        {
            _properties = values.ToDictionary(x => x.Key, x => x.Value, StringComparer.InvariantCultureIgnoreCase);
        }

        public string Id { get { return GetValue(_idName); } set { _properties[_idName] = value; } }
        public string Cv { get { return GetValue(_cvName); } set { _properties[_cvName] = value; } }
        public string ActivityId { get { return GetValue(_activityIdName); } set { _properties[_activityIdName] = value; } }
        public string ParentActivityId { get { return GetValue(_parentActivityIdName); } set { _properties[_parentActivityIdName] = value; } }

        public EventDimensionsBuilder Set(string key, string value)
        {
            _properties[key] = value;
            return this;
        }

        public EventDimensionsBuilder Clear()
        {
            _properties.Clear();
            return this;
        }

        public EventDimensionsBuilder Remove(string key)
        {
            _properties.Remove(key);
            return this;
        }

        public IEventDimensions Build()
        {
            return _properties
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.InvariantCultureIgnoreCase)
                .AsTypeOrDefault<IEventDimensions>();
        }

        private string GetValue(string key)
        {
            if( !_properties.TryGetValue(key, out string value))
            {
                return null;
            }

            return value;
        }
    }
}
