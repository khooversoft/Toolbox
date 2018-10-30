using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb.Models.V1
{
    [DebuggerDisplay("Name={Name}, SizeOnDisk={SizeOnDisk}, Empty={Empty}")]
    public class DatabaseDetailV1
    {
        public string Name { get; set; }

        public double SizeOnDisk { get; set; }

        public bool Empty { get; set; }
    }
}
