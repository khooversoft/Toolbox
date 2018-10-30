using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Khooversoft.Toolbox;
using MongoDB.Bson;

namespace Khooversoft.MongoDb
{
    public class Projection : IInstructionNode, IEnumerable<string>
    {
        private readonly IList<string> _list;

        public Projection()
        {
            _list = new List<string>();
        }

        public Projection(IEnumerable<string> values)
        {
            Verify.IsNotNull(nameof(values), values);

            _list = values.ToList();
        }

        public Projection Clear()
        {
            _list.Clear();
            return this;
        }

        public Projection Add(string key)
        {
            _list.Add(key);
            return this;
        }

        public BsonDocument ToDocument()
        {
            return new BsonDocument(_list.Select(x => new KeyValuePair<string, object>(x, 1)));
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
