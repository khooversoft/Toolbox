﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class Query : InstructionCollection<IInstructionNode>, IInstructionNode
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

        public static Query operator +(Query rootNode, IInstructionNode nodeToAdd)
        {
            rootNode.Add(nodeToAdd);
            return rootNode;
        }
    }
}
