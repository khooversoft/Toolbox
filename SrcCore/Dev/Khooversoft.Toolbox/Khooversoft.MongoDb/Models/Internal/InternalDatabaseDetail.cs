using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    internal class InternalDatabaseDetail
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("sizeOnDisk")]
        public double SizeOnDisk { get; set; }

        [BsonElement("empty")]
        public bool Empty { get; set; }
    }
}
