using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox
{
    public static class WorkContextExtensions
    {
        /// <summary>
        /// Convert key value pairs to Properties
        /// </summary>
        /// <param name="self">list</param>
        /// <returns>properties</returns>
        public static IProperties ToProperties(this IEnumerable<KeyValuePair<string, object>> self)
        {
            if (self == null)
            {
                return null;
            }

            return self
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.InvariantCultureIgnoreCase)
                .AsTypeOrDefault<IProperties>();
        }
    }
}
