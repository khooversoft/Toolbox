using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox
{
    public class Properties : Dictionary<string, object>, IProperties
    {
        public Properties()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public Properties(IEnumerable<KeyValuePair<string, object>> values)
            : base(values, StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public static IProperties Empty { get; } = new Properties();
    }
}
