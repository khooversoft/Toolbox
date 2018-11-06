using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MongoDb
{
    public class Aggregate : InstructionCollection<IAggregateNode>
    {
        public Aggregate()
        {
        }

        public BsonDocument[] ToDocuments()
        {
            var list = new List<BsonDocument>();

            foreach (var item in this)
            {
                list.Add(item.ToDocument());
            }

            return list.ToArray();
        }

        public static Aggregate operator +(Aggregate rootNode, IAggregateNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
