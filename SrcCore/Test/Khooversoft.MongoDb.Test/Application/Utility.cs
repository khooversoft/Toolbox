using FluentAssertions;
using Khooversoft.Toolbox;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb.Test
{
    internal static class Utility
    {
        private const string _dbName = "TestQueryDatabase";
        private const string _collectionName = "TestCollection";
        private static readonly IWorkContext _workContext = WorkContext.Empty;
        private static readonly object _lock = new object();

        public static void Initialize()
        {
            lock (_lock)
            {
                if (DocumentServer != null)
                {
                    return;
                }

                TranData = DateTime.UtcNow;

                DocumentServer = new DocumentServer(Constants.ConnectionString);

                TestDocuments = Enumerable.Range(0, Count)
                    .Select(x => Utility.CreateTestDocument(TranData, x))
                    .OrderBy(x => x._id)
                    .ToList();

                Database = DocumentServer.GetDatabase(_workContext, _dbName);
                Collection = Database.GetCollection<TestDocument>(_collectionName);

                ResetDatabase()
                    .GetAwaiter()
                    .GetResult();
            }
        }

        public static int Count = 10;

        public static IDocumentServer DocumentServer { get; private set; }

        public static List<TestDocument> TestDocuments { get; private set; }

        public static IDocumentDatabase Database { get; private set; }

        public static IDocumentCollection<TestDocument> Collection { get; private set; }

        public static DateTime TranData { get; private set; }

        private static async Task ResetDatabase()
        {
            await DocumentServer.DropDatabase(_workContext, _dbName);
            await TestDocuments.RunAsync(async x => await Collection.Insert(_workContext, x));
        }

        public static TestDocument CreateTestDocument(DateTime tranDate, int index)
        {
            DateTime tt = tranDate.AddDays(index);

            return new TestDocument
            {
                Index = index,
                FirstName = $"First_{index}",
                LastName = $"Last_{index}",
                Birthdate = DateTime.UtcNow,
                Address1 = $"Addr1_{index}",
                Address2 = $"Addr2_{index}",
                City = $"City_{index}",
                State = $"State_{index}",
                ZipCode = $"Zip_{index}",
                TranDate = tt,
                GuidKey = Guid.NewGuid(),
                LockedDate = index % 3 == 0 ? tt : (DateTime?)null,
            };
        }

        internal static void VerifyDocuments(IEnumerable<TestDocument> sourceDocuments, IEnumerable<TestDocument> resultDocuments, bool orderData = true)
        {
            int docIndex = 0;
            var sourceList = orderData ? sourceDocuments.OrderBy(x => x.Index).ToList() : sourceDocuments.ToList();
            var resultList = orderData ? resultDocuments.OrderBy(x => x.Index).ToList() : sourceDocuments.ToList();

            sourceList.Count.Should().Be(resultList.Count);

            foreach (var result in resultDocuments)
            {
                var test = sourceList[docIndex++];

                result.FirstName.Should().Be(test.FirstName);
                result.LastName.Should().Be(test.LastName);
                result.Birthdate.ToString("s").Should().Be(test.Birthdate.ToString("s"));
                result.Address1.Should().Be(test.Address1);
                result.Address2.Should().Be(test.Address2);
                result.City.Should().Be(test.City);
                result.State.Should().Be(test.State);
                result.ZipCode.Should().Be(test.ZipCode);
                result.TranDate.Should().Be(test.TranDate);
                result.GuidKey.Should().Be(test.GuidKey);
                result.LockedDate.Should().Be(test.LockedDate);
            }
        }
    }
}
