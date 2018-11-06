using Khooversoft.Toolbox;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class OrderBy : IInstructionNode
    {
        public OrderBy(DirectionType direction, string name)
        {
            Verify.IsNotEmpty(nameof(name), name);

            Direction = direction;
            Name = name;
        }

        public DirectionType Direction { get; }

        public string Name { get; }

        public BsonDocument ToDocument()
        {
            int order = 0;
            switch (Direction)
            {
                case DirectionType.Ascending:
                    order = 1;
                    break;

                case DirectionType.Descending:
                    order = -1;
                    break;

                default:
                    throw new ArgumentException($"Unknown compare type: {Direction}");
            }

            return new BsonDocument(Name, BsonValue.Create(order));
        }
    }
}
