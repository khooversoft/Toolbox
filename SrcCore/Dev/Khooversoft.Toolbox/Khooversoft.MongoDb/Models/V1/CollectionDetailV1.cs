using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb.Models.V1
{
    public class CollectionDetailV1
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public bool Readonly { get; set; }

        public bool Capped { get; set; }

        public int? MaxDocuments { get; set; }

        public long? MaxSizeInBytes { get; set; }
    }
}
