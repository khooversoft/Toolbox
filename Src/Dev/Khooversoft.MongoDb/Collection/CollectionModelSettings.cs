using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class CollectionModelSettings
    {
        public bool Remove { get; set; }

        public bool ReCreate { get; set; }

        public bool AllowDataLoss { get; set; }
    }
}
