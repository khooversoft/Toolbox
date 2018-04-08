using FluentAssertions;
using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.MongoDb.Test.DatabaseTests
{
    [Trait("Category", "Toolbox")]
    public class CollectionModelPackageTests
    {
        private readonly IDocumentServer _documentServer;
        private readonly IWorkContext _workContext = WorkContext.Empty;
        private static readonly string _dbName = $"TestIndexDatabase_{nameof(CollectionModelPackageTests)}";
        private const string _collectionName = "TestCollection";

        public CollectionModelPackageTests()
        {
            _documentServer = new DocumentServer(Constants.ConnectionString);

            _documentServer.DropDatabase(_workContext, _dbName)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Test goals:
        ///   Limited help testers
        ///   Create or recreate collection model
        ///   Verify model
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ApplyCollectionModelFull()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);

            var model = new CollectionModel
            {
                CollectionName = _collectionName,
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

            var package = new CollectionModelPackage(db, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            (await db.CollectionExist(_workContext, _collectionName)).Should().BeTrue();
            IDocumentCollection<TestDocument> collection = db.GetCollection<TestDocument>(_collectionName);

            IndexDetailV1 detail = await collection.Index.GetIndexDetail(_workContext, model.Indexes[0].Name);
            detail.Should().NotBeNull();
            detail.Keys.Count.Should().Be(1);

            model.Indexes[0].IsEquals(detail).Should().BeTrue();

            await db.DropCollection(_workContext, _collectionName);
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        [Fact]
        public async Task ApplyCollectionModelMultipleIndexesFull()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);

            CollectionModel model = CreateCollectionModel(2);

            var package = new CollectionModelPackage(db, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            (await VerifyCollectionModel(db, model)).Should().BeTrue();

            await db.DropCollection(_workContext, _collectionName);
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        [Fact]
        public async Task ApplyCollectionModelCompositeKeyFull()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);

            var model = new CollectionModel
            {
                CollectionName = _collectionName,
                Indexes = new List<CollectionIndex>
                {
                    new CollectionIndex
                    {
                        Name = "TestIndex_1",
                        Unique = true,
                        Keys = new List<IndexKey>
                        {
                            new IndexKey { FieldName = "Field1", Descending = false },
                            new IndexKey { FieldName = "Field2", Descending = true },
                        }
                    }
                }
            };

            var package = new CollectionModelPackage(db, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            (await VerifyCollectionModel(db, model)).Should().BeTrue();

            await db.DropCollection(_workContext, _collectionName);
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        [Fact]
        public async Task TestCollectionModelFull()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);

            CollectionModel model = CreateCollectionModel(1);

            var package = new CollectionModelPackage(db, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            package = new CollectionModelPackage(db, model, new CollectionModelSettings());
            result = await package.Test(_workContext);
            result.Should().BeTrue();

            await db.DropCollection(_workContext, _collectionName);
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        [Fact]
        public async Task TestCollectionModelFullFail()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);

            foreach (var variation in Enumerable.Range(1, 2))
            {
                CollectionModel model = CreateCollectionModel(variation);

                var package = new CollectionModelPackage(db, model, new CollectionModelSettings { ReCreate = true });

                bool result = await package.Apply(_workContext);
                result.Should().BeTrue();

                package = new CollectionModelPackage(db, model, new CollectionModelSettings());
                result = await package.Test(_workContext);
                result.Should().BeTrue();

                model.Indexes[0].Keys.Add(new IndexKey { FieldName = "xxxx" });
                result = await package.Test(_workContext);
                result.Should().BeFalse();

                await db.DropCollection(_workContext, _collectionName);
            }

            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        [Fact]
        public async Task ApplyCollectionMultipleIndexesFull()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);

            CollectionModel model = CreateCollectionModel(2);

            var package = new CollectionModelPackage(db, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            (await VerifyCollectionModel(db, model)).Should().BeTrue();

            await db.DropCollection(_workContext, _collectionName);
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        [Fact]
        public async Task ApplyUpdateIncreaseIndexAndVerify()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);

            CollectionModel model = CreateCollectionModel(2);

            var package = new CollectionModelPackage(db, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            (await VerifyCollectionModel(db, model)).Should().BeTrue();

            model = CreateCollectionModel(3);

            var updatePackage = new CollectionModelPackage(db, model, new CollectionModelSettings());
            result = await updatePackage.Apply(_workContext);
            result.Should().BeTrue();

            (await VerifyCollectionModel(db, model)).Should().BeTrue();

            await db.DropCollection(_workContext, _collectionName);
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        [Fact]
        public async Task ApplyUpdateRemovedIndexUpdateModel()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);

            CollectionModel model = CreateCollectionModel(3);

            var package = new CollectionModelPackage(db, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            (await VerifyCollectionModel(db, model)).Should().BeTrue();

            model = CreateCollectionModel(2);

            var updatePackage = new CollectionModelPackage(db, model, new CollectionModelSettings());
            result = await updatePackage.Apply(_workContext);
            result.Should().BeTrue();

            (await VerifyCollectionModel(db, model)).Should().BeTrue();

            await db.DropCollection(_workContext, _collectionName);
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        [Fact]
        public async Task ApplyUpdateRemovedCollection()
        {
            DocumentDatabase db = _documentServer.GetDatabase(_workContext, _dbName);

            CollectionModel model = CreateCollectionModel(3);

            var package = new CollectionModelPackage(db, model, new CollectionModelSettings { ReCreate = true });

            bool result = await package.Apply(_workContext);
            result.Should().BeTrue();

            (await VerifyCollectionModel(db, model)).Should().BeTrue();

            model = CreateCollectionModel(0);

            var updatePackage = new CollectionModelPackage(db, model, new CollectionModelSettings { Remove = true });
            result = await updatePackage.Apply(_workContext);
            result.Should().BeTrue();

            (await db.CollectionExist(_workContext, _collectionName)).Should().BeFalse();

            await db.DropCollection(_workContext, _collectionName);
            await _documentServer.DropDatabase(_workContext, _dbName);
        }

        private class TestDocument
        {
            public string FirstName { get; set; }
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

        private async Task<bool> VerifyCollectionModel(DocumentDatabase db, CollectionModel model)
        {
            (await db.CollectionExist(_workContext, _collectionName)).Should().BeTrue();
            IDocumentCollection<TestDocument> collection = db.GetCollection<TestDocument>(_collectionName);

            foreach (var indexDetail in model.Indexes)
            {
                IndexDetailV1 detail = await collection.Index.GetIndexDetail(_workContext, indexDetail.Name);
                detail.Should().NotBeNull();

                if (!indexDetail.IsEquals(detail))
                {
                    return false;
                }
            }

            IEnumerable<IndexDetailV1> dbIndex = await collection.Index.ListIndexes(_workContext);
            dbIndex.Should().NotBeNull();
            dbIndex.Count().Should().Be(model.Indexes.Count + 1);

            return dbIndex
                .Where(x => x.Name != "_id_")
                .OrderBy(x => x.Name)
                .Zip(model.Indexes.OrderBy(x => x.Name), (d, m) => new { Db = d, Model = m })
                .All(x => x.Model.IsEquals(x.Db));
        }
    }
}
