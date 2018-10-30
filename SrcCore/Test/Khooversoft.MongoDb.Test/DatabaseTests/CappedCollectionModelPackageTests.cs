// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.MongoDb.Test.DatabaseTests
{
    [Trait("Category", "Toolbox")]
    public class CappedCollectionModelPackageTests
    {
        private readonly IDocumentServer _documentServer;
        private readonly IWorkContext _workContext = WorkContext.Empty;
        private readonly IDocumentDatabase _documentDatabase;
        private const string _collectionName = "CappedTestCollection";

        public CappedCollectionModelPackageTests()
        {
            string databaseName = $"TestCappedDatabase_{nameof(CappedCollectionModelPackageTests)}";

            _documentServer = new DocumentServer(Constants.ConnectionString);

            _documentServer
                .DropDatabase(_workContext, databaseName)
                .GetAwaiter()
                .GetResult();

            _documentDatabase = new DocumentDatabase(new DatabaseConfigurationBuilder(Constants.ConnectionString, databaseName).ToString());
        }

        [Fact]
        public async Task ApplyCappedCollectionModelWithoutIndex()
        {
            const int maxDocuments = 100;
            const int maxSizeInBytes = maxDocuments * 1000;

            var model = new CappedCollectionModel
            {
                CollectionName = _collectionName,
                MaxSizeInBytes = maxSizeInBytes,
                MaxNumberOfDocuments = maxDocuments,
            };

            var package = new CollectionModelPackage(_documentDatabase, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            (await _documentDatabase.CollectionExist(_workContext, _collectionName)).Should().BeTrue();
            CollectionDetailV1 detail = await _documentDatabase.GetCollectionDetail(_workContext, _collectionName);
            detail.Should().NotBeNull();
            detail.Name.Should().Be(_collectionName);
            detail.Type.Should().Be("collection");
            detail.Readonly.Should().BeFalse();
            detail.Capped.Should().BeTrue();
            detail.MaxDocuments.Should().HaveValue();
            detail.MaxDocuments.Should().Be(maxDocuments);
            detail.MaxSizeInBytes.Should().HaveValue();
            detail.MaxSizeInBytes.Should().BeGreaterOrEqualTo(maxSizeInBytes);
        }

        [Fact]
        public async Task ApplyCappedCollectionModelWithIndex()
        {
            const int maxDocuments = 100;
            const int maxSizeInBytes = maxDocuments * 1000;

            var model = new CappedCollectionModel
            {
                CollectionName = _collectionName,
                MaxSizeInBytes = maxSizeInBytes,
                MaxNumberOfDocuments = maxDocuments,
                Indexes = new List<CollectionIndex>
                {
                    new CollectionIndex
                    {
                        Name = "TestIndex_1",
                        Unique = true,
                        Keys = new List<IndexKey>
                        {
                            new IndexKey { FieldName = "Field1", Descending = false },
                        }
                    }
                }
            };

            var package = new CollectionModelPackage(_documentDatabase, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            (await _documentDatabase.CollectionExist(_workContext, _collectionName)).Should().BeTrue();
            CollectionDetailV1 detail = await _documentDatabase.GetCollectionDetail(_workContext, _collectionName);
            detail.Should().NotBeNull();
            detail.Name.Should().Be(_collectionName);
            detail.Type.Should().Be("collection");
            detail.Readonly.Should().BeFalse();
            detail.Capped.Should().BeTrue();
            detail.MaxDocuments.Should().HaveValue();
            detail.MaxDocuments.Should().Be(maxDocuments);
            detail.MaxSizeInBytes.Should().HaveValue();
            detail.MaxSizeInBytes.Should().BeGreaterOrEqualTo(maxSizeInBytes);

            IDocumentCollection<TestDocument> collection = _documentDatabase.GetCollection<TestDocument>(_collectionName);
            IndexDetailV1 indexDetail = await collection.Index.GetIndexDetail(_workContext, model.Indexes[0].Name);
            indexDetail.Should().NotBeNull();
            indexDetail.Keys.Count.Should().Be(1);

            model.Indexes[0].IsEquals(indexDetail).Should().BeTrue();
        }

        [Fact]
        public async Task SmallCappedCollectionTest()
        {
            const int maxDocuments = 10;
            const int maxSizeInBytes = maxDocuments * 1000;

            var model = new CappedCollectionModel
            {
                CollectionName = _collectionName,
                MaxSizeInBytes = maxSizeInBytes,
                MaxNumberOfDocuments = maxDocuments,
            };

            var package = new CollectionModelPackage(_documentDatabase, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            IDocumentCollection<TestDocument> collection = _documentDatabase.GetCollection<TestDocument>(_collectionName);

            foreach (var index in Enumerable.Range(0, maxDocuments))
            {
                TestDocument doc = CreateTestDocument(index);
                await collection.Insert(_workContext, doc);
            }

            long count = await collection.Count(_workContext);
            count.Should().Be(maxDocuments);

            IEnumerable<TestDocument> results = await collection.Find(_workContext, new BsonDocument());
            results.Should().NotBeNull();
            results.Count().Should().Be(maxDocuments);

            int testIndex = 0;
            foreach (var item in results.OrderBy(x => x.Index))
            {
                TestDocument compareDoc = CreateTestDocument(testIndex++);
                item.IsEqual(compareDoc).Should().BeTrue();
            }
        }

        [Fact]
        public async Task SmallCappedOverFiveCollectionTest()
        {
            const int maxDocuments = 10;
            const int maxSizeInBytes = maxDocuments * 1000;
            const int createDocumentCount = maxDocuments + 5;

            var model = new CappedCollectionModel
            {
                CollectionName = _collectionName,
                MaxSizeInBytes = maxSizeInBytes,
                MaxNumberOfDocuments = maxDocuments,
            };

            var package = new CollectionModelPackage(_documentDatabase, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            IDocumentCollection<TestDocument> collection = _documentDatabase.GetCollection<TestDocument>(_collectionName);

            foreach (var index in Enumerable.Range(0, createDocumentCount))
            {
                TestDocument doc = CreateTestDocument(index);
                await collection.Insert(_workContext, doc);
            }

            long count = await collection.Count(_workContext);
            count.Should().Be(maxDocuments);

            IEnumerable<TestDocument> results = await collection.Find(_workContext, new BsonDocument());
            results.Should().NotBeNull();
            results.Count().Should().Be(maxDocuments);

            int testIndex = createDocumentCount - maxDocuments;
            foreach (var item in results.OrderBy(x => x.Index))
            {
                TestDocument compareDoc = CreateTestDocument(testIndex++);
                item.IsEqual(compareDoc).Should().BeTrue();
            }
        }

        private TestDocument CreateTestDocument(int index)
        {
            return new TestDocument
            {
                Index = index,
                FirstName = $"First_{index}",
                LastName = $"Last_{index}",
            };
        }

        private class TestDocument
        {
            public ObjectId _id { get; set; }

            public int Index { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public bool IsEqual(TestDocument document)
            {
                return Index == document.Index &&
                    FirstName == document.FirstName &&
                    LastName == document.LastName;
            }
        }

        private CollectionModel CreateCollectionModel(int numberOfIndexes)
        {
            int numberOfKeys = 1;
            return new CollectionModel
            {
                CollectionName = _collectionName,
                Indexes = Enumerable.Range(0, numberOfIndexes)
                    .Select(x => new CollectionIndex
                    {
                        Name = $"TestIndex_{x}",
                        Unique = true,
                        Keys = Enumerable.Range(0, numberOfKeys++).Select(y => new IndexKey { FieldName = $"Field_{y}" }).ToList(),
                    }).ToList(),
            };
        }
    }
}
