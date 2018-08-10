using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class And : InstructionCollection, IInstructionNode
    {
        public BsonDocument ToDocument()
        {
            var array = new BsonArray();
            foreach(var item in this)
            {
                array.Add(item.ToDocument());
            }

            return new BsonDocument("$and", array);
        }

        public static And operator +(And rootNode, IInstructionNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
