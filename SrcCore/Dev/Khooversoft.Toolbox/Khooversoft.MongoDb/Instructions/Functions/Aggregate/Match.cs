using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MongoDb
{
    public class Match : InstructionCollection<IInstructionNode>, IAggregateNode
    {
        public BsonDocument ToDocument()
        {
            var doc = new BsonDocument();

            foreach (var item in this)
            {
                doc.AddRange(item.ToDocument());
            }

            return new BsonDocument("$match", doc);
        }

        public static Match operator +(Match rootNode, IInstructionNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
