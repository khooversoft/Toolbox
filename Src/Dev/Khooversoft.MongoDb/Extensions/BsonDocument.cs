using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Khooversoft.Toolbox;
using MongoDB.Bson;

namespace Khooversoft.MongoDb
{
    public static class BsonDocument_Extension
    {
        /// <summary>
        /// Try to get element base on path
        /// </summary>
        /// <param name="document">this document</param>
        /// <param name="path">path to element (i.e. "root.child")</param>
        /// <returns>Element</returns>
        public static BsonElement? TryGetElementAt(this BsonDocument document, string path)
        {
            Verify.IsNotNull(nameof(document), document);
            Verify.IsNotEmpty(nameof(path), path);

            BsonDocument currentDocument = document;
            BsonElement element;

            Stack<string> pathParts = new Stack<string>(path.Split('.').Reverse());

            while (pathParts.Count > 0)
            {
                string item = pathParts.Pop();

                if (currentDocument.TryGetElement(item, out element) == false || (pathParts.Count != 0 && element.Value.IsBsonDocument == false)) break;

                if (pathParts.Count == 0) return element;

                currentDocument = element.Value.ToBsonDocument();
            }

            return null;
        }

        /// <summary>
        /// Get element base on path
        /// </summary>
        /// <param name="document">this document</param>
        /// <param name="path">path to element (i.e. "root.child")</param>
        /// <returns>Element</returns>
        public static BsonElement GetElementAt(this BsonDocument document, string path)
        {
            BsonElement? element = document.TryGetElementAt(path);
            if (element.HasValue == false)
            {
                throw new ArgumentException("BsonDocument does not have path", path);
            }

            return element.Value;
        }

        /// <summary>
        /// Convert to json strict
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="obj">object</param>
        /// <returns>Json string</returns>
        public static string ToJsonStrict<T>(this T obj)
        {
            var settings = new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict };
            return obj.ToJson(settings);
        }

        /// <summary>
        /// Convert type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Get<T>(this BsonDocument self, string name, T defaultValue = default(T), bool required = true)
        {
            Verify.IsNotNull(nameof(self), self);
            Verify.IsNotEmpty(nameof(name), name);

            if (!self.Contains(name))
            {
                if (required)
                {
                    throw new InvalidOperationException($"Field {name} does not exist and required");
                }

                return defaultValue;
            }

            BsonValue value = self[name];

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType((string)value, typeof(T));
            }
            if (typeof(T) == typeof(bool))
            {
                return (T)Convert.ChangeType((bool)value, typeof(T));
            }
            if (typeof(T) == typeof(bool?))
            {
                return (T)Convert.ChangeType((bool?)value, typeof(T));
            }
            if (typeof(T) == typeof(int))
            {
                return (T)Convert.ChangeType((int)value, typeof(T));
            }
            if (typeof(T) == typeof(int?))
            {
                return (T)Convert.ChangeType((int?)value, typeof(T));
            }
            if (typeof(T) == typeof(long))
            {
                return (T)Convert.ChangeType((long)value, typeof(T));
            }
            if (typeof(T) == typeof(long?))
            {
                return (T)Convert.ChangeType((long?)value, typeof(T));
            }
            if (typeof(T) == typeof(decimal))
            {
                return (T)Convert.ChangeType((decimal)value, typeof(T));
            }
            if (typeof(T) == typeof(decimal?))
            {
                return (T)Convert.ChangeType((decimal?)value, typeof(T));
            }
            if (typeof(T) == typeof(DateTime))
            {
                return (T)Convert.ChangeType((DateTime)value, typeof(T));
            }
            if (typeof(T) == typeof(DateTime?))
            {
                return (T)Convert.ChangeType((DateTime?)value, typeof(T));
            }
            if (typeof(T) == typeof(Guid))
            {
                return (T)Convert.ChangeType((Guid)value, typeof(T));
            }
            if (typeof(T) == typeof(Guid?))
            {
                return (T)Convert.ChangeType((Guid?)value, typeof(T));
            }

            throw new InvalidOperationException($"Unsupported type: {value.GetType()}");
        }
    }
}
