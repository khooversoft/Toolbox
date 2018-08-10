using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Khooversoft.MongoDb
{
    public class Or : InstructionCollection, IInstructionNode
    {
        public Or()
        {
        }

        public BsonDocument ToDocument()
        {
            var array = new BsonArray();

            foreach(var item in this)
            {
                array.Add(item.ToDocument());
            }

            return new BsonDocument("$or", array);
        }

        public static Or operator +(Or rootNode, IInstructionNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
