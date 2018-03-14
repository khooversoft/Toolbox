using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Page request based on index
    /// </summary>
    public class PageRequest
    {
        public PageRequest(int limit, string index = null)
        {
            Verify.Assert(limit > 0, nameof(limit));

            Limit = limit;
            Index = index;
        }

        /// <summary>
        /// Maximum limit to return data set
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Index for next page (optional)
        /// </summary>
        public string Index { get; }
    }
}
