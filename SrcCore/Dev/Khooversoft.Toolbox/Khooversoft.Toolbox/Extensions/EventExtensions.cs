using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox
{
    public static class EventExtensions
    {
        /// <summary>
        /// Create new event dimensions by merging two instances.  "newOrOverrides" will take
        /// precedent of parent values
        /// </summary>
        /// <param name="source">source of dimension values</param>
        /// <param name="upserts">new or updates to dimension values</param>
        /// <returns>merged new event dimensions</returns>
        //public static IEventDimensions WithUpserts(this IEventDimensions source, IEventDimensions upserts)
        //{
        //    if (source == null)
        //    {
        //        return null;
        //    }

        //    return source
        //        .Select(x => new { Index = 2, Dim = x })
        //        .Concat(upserts.Select(x => new { Index = 1, Dim = x }))
        //        .GroupBy(x => x.Dim.Key)
        //        .Select(x => x.OrderBy(y => y.Index).First())
        //        .ToDictionary(x => x.Dim.Key, x => x.Dim.Value, StringComparer.InvariantCultureIgnoreCase)
        //        .AsTypeOrDefault<IEventDimensions>();
        //}

        /// <summary>
        /// Convert key value pairs to Event Dimensions
        /// </summary>
        /// <param name="self">list</param>
        /// <returns>Event Dimensions</returns>
        public static IEventDimensions ToEventDimensions(this IEnumerable<KeyValuePair<string, string>> self)
        {
            if (self == null)
            {
                return null;
            }

            return self
                .ToList()
                .AsType<IEventDimensions>();
        }
    }
}
