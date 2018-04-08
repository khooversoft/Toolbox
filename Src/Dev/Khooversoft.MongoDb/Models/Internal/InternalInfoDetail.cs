using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    [BsonIgnoreExtraElements]
    public class InternalInfoDetail
    {
        [BsonElement("readOnly")]
        public bool Readonly { get; set; }

        //[BsonElement("uuid")]
        //[BsonId]
        //public ObjectId Uuid { get; set; }
    }
}
