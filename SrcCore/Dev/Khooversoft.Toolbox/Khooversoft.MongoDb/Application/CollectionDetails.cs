using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    [DebuggerDisplay("CollectionName={CollectionName}, NumberOfDocuments={NumberOfDocuments}")]
    public class CollectionDetails
    {
        /// <summary>
        /// Collection name
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Number of documents
        /// </summary>
        public int NumberOfDocuments { get; set; }
    }
}
