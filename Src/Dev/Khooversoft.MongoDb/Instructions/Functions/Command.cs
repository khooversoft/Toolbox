using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Khooversoft.MongoDb
{
    public class Command : InstructionCollection<ICommandNode>, ICommandNode
    {
        public BsonDocument ToDocument()
        {
            var doc = new BsonDocument();

            foreach (var item in this)
            {
                doc.AddRange(item.ToDocument());
            }

            return doc;
        }

        public static Command operator +(Command rootNode, ICommandNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
