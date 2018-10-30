using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MongoDb.Test
{
    internal class TestDocument
    {
        public ObjectId _id { get; set; }

        public int Index { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Birthdate { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.Int64)]
        public DateTime TranDate { get; set; }

        public Guid GuidKey { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.Int64)]
        public DateTime? LockedDate { get; set; }
    }
}
