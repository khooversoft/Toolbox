using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Khooversoft.Toolbox
{
    public static class EventDimensionsExtensions
    {
        public static IEnumerable<KeyValuePair<string, object>> Materialized(this IWorkContext context, string message, IEnumerable<KeyValuePair<string, object>> dimensions)
        {
            Verify.IsNotNull(nameof(context), context);

            return context.Dimensions
                .Concat(dimensions ?? Enumerable.Empty<KeyValuePair<string, object>>())
                .Concat(new KeyValuePair<string, object>("message", message).ToEnumerable())
                .Materialized()
                .Where(x => x.Value != null);
        }

        public static IEnumerable<KeyValuePair<string, object>> Materialized(this IWorkContext context, IEnumerable<KeyValuePair<string, object>> dimensions)
        {
            Verify.IsNotNull(nameof(context), context);

            return context.Dimensions
                .Concat(dimensions ?? Enumerable.Empty<KeyValuePair<string, object>>())
                .Materialized()
                .Where(x => x.Value != null);
        }

        public static IEnumerable<KeyValuePair<string, object>> Materialized(this IWorkContext context, string message, object dimensions)
        {
            Verify.IsNotNull(nameof(context), context);

            if( dimensions== null)
            {
                return context.Dimensions
                    .Materialized()
                    .Where(x => x.Value != null);
            }

            return context.Dimensions
                .Concat(dimensions?.ToKeyValues() ?? Enumerable.Empty<KeyValuePair<string, object>>())
                .Concat(new KeyValuePair<string, object>("message", message).ToEnumerable())
                .Materialized()
                .Where(x => x.Value != null);
        }

        public static IEnumerable<KeyValuePair<string, object>> Materialized(this IWorkContext context, object dimensions)
        {
            Verify.IsNotNull(nameof(context), context);

            if (dimensions == null)
            {
                return context.Dimensions
                    .Materialized()
                    .Where(x => x.Value != null);
            }

            return context.Dimensions
                .Concat(dimensions?.ToKeyValues() ?? Enumerable.Empty<KeyValuePair<string, object>>())
                .Materialized()
                .Where(x => x.Value != null);
        }
    }
}
