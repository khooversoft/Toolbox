using Khooversoft.Toolbox.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public static class EnumerableExtension
    {
        public static string ToDelimitedString<T>(this IEnumerable<T> self, string delimiter = ", ")
        {
            if( self == null )
            {
                return string.Empty;
            }

            return string.Join(delimiter, self.Select(x => x.ToString()));
        }
    }
}
