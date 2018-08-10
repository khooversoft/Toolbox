using Khooversoft.Toolbox;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class Compare : IInstructionNode
    {
        public Compare(CompareType compareType, string name, object value)
        {
            Verify.IsNotEmpty(nameof(name), name);

            CompareType = compareType;
            Name = name;
            Value = value;
        }

        public CompareType CompareType { get; }

        public string Name { get; }

        public object Value { get; }

        public BsonDocument ToDocument()
        {
            string symbol;
            switch(CompareType )
            {
                case CompareType.Equal:
                    symbol = "$eq";
                    break;

                case CompareType.GreaterThen:
                    symbol = "$gt";
                    break;

                case CompareType.GreaterThenEqual:
                    symbol = "$gte";
                    break;

                case CompareType.LessThen:
                    symbol = "$lt";
                    break;

                case CompareType.LessThenEqual:
                    symbol = "$lte";
                    break;

                case CompareType.NotEqual:
                    symbol = "$ne";
                    break;

                default:
                    throw new ArgumentException($"Unknown compare type: {CompareType}");
            }

            object v = Value;
            switch(Value)
            {
                case DateTime dateTime:
                    v = dateTime.Ticks;
                    return new BsonDocument(Name, new BsonDocument(symbol, BsonValue.Create(v)));

                default:
                    return new BsonDocument(Name, new BsonDocument(symbol, BsonValue.Create(Value)));
            }
        }
    }
}
