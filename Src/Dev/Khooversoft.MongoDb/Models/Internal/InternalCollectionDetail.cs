using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    [BsonIgnoreExtraElements]
    public class InternalCollectionDetail
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("info")]
        public InternalInfoDetail Info { get; set; }
    }
}
