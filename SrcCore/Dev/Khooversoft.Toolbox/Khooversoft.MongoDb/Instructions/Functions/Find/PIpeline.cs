using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MongoDb
{
    public class Pipeline : InstructionCollection<IInstructionNode>, IInstructionCollection
    {
        public Pipeline()
        {
        }

        public BsonDocument ToDocument()
        {
            var doc = new BsonDocument();

            foreach (var item in this)
            {
                doc.AddRange(item.ToDocument());
            }

            return doc;
        }

        public static Pipeline operator +(Pipeline rootNode, IInstructionNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
