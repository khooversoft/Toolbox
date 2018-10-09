using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox
{
    public class PropertyBag : Dictionary<string, object>, IPropertyBag
    {
        public PropertyBag()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public PropertyBag(IEnumerable<KeyValuePair<string, object>> values)
            : base(values, StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public static IPropertyBag Empty { get; } = new PropertyBag();
    }
}
