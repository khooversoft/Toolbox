using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class Field : InstructionCollection, IInstructionNode
    {
        public Field(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; }

        public BsonDocument ToDocument()
        {
            var array = new BsonArray();
            foreach (var item in this)
            {
                array.Add(item.ToDocument());
            }

            return new BsonDocument("$and", array);
        }

        public override int GetHashCode()
        {
            return FieldName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Field))
            {
                return false;
            }

            return FieldName.Equals(((Field)obj).FieldName);
        }

        public static Field operator +(Field field, IInstructionNode nodeToAdd)
        {
            field.Add(nodeToAdd);
            return field;
        }

        public static Field operator ==(Field field, object value)
        {
            field.Add(new Compare(CompareType.Equal, field.FieldName, value));
            return field;
        }

        public static Field operator !=(Field field, object value)
        {
            field.Add(new Compare(CompareType.NotEqual, field.FieldName, value));
            return field;
        }

        public static Field operator <(Field field, object value)
        {
            field.Add(new Compare(CompareType.LessThen, field.FieldName, value));
            return field;
        }

        public static Field operator >(Field field, object value)
        {
            field.Add(new Compare(CompareType.GreaterThen, field.FieldName, value));
            return field;
        }

        public static Field operator <=(Field field, object value)
        {
            field.Add(new Compare(CompareType.LessThenEqual, field.FieldName, value));
            return field;
        }

        public static Field operator >=(Field field, object value)
        {
            field.Add(new Compare(CompareType.GreaterThenEqual, field.FieldName, value));
            return field;
        }
    }
}
