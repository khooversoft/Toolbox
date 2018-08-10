using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.MongoDb.Test.DatabaseTests
{
    [Trait("Category", "MongoDB")]
    public class BsonTest
    {
        [Fact]
        public void Test()
        {
            var doc = new BsonDocument();

            var array = new BsonArray();
            var element1 = new BsonElement("name1", BsonValue.Create("value1"));
            var element2 = new BsonElement("name2", BsonValue.Create("value2"));
            //var doc1 = new BsonDocument();
            //doc1.AddRange(new BsonDocument(element1));
            //doc1.Add(element2);
            array.Add(new BsonDocument(element1));
            array.Add(new BsonDocument(element2));

            doc.Add("$cmd", array);
        }

        [Fact]
        public void Test1()
        {
            var doc = new BsonDocument();
            doc.Add("key1", new BsonDocument().Add("subkey1", "subvalue1"));
            doc.Add("key2", "value2");
        }
    }
}
