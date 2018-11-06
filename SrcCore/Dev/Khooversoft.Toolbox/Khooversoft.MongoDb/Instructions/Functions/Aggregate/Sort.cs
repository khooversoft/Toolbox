using Khooversoft.Toolbox;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class Sort : InstructionCollection<IInstructionNode>, IAggregateNode
    {
        public BsonDocument ToDocument()
        {
            var doc = new BsonDocument();

            foreach (var item in this)
            {
                doc.AddRange(item.ToDocument());
            }

            return new BsonDocument("$sort", doc);
        }

        public static Sort operator +(Sort rootNode, IInstructionNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
