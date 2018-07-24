﻿using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class DocumentCollection<TDocument> : IDocumentCollection<TDocument>
    {
        public DocumentCollection(IMongoCollection<TDocument> mongoCollection, string collectionName)
        {
            Verify.IsNotNull(nameof(mongoCollection), mongoCollection);
            Verify.IsNotEmpty(nameof(collectionName), collectionName);

            MongoCollection = mongoCollection;
            CollectionName = collectionName;
            Index = new DocumentIndex<TDocument>(this);
        }

        public string CollectionName { get; }

        public IMongoCollection<TDocument> MongoCollection { get; }

        public IDocumentIndex<TDocument> Index { get; }

        public async Task Insert(IWorkContext context, TDocument document)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(document), document);

            var options = new InsertOneOptions();

            await MongoCollection.InsertOneAsync(document, options, context.CancellationToken);
        }

        public async Task<IEnumerable<TDocument>> Find(IWorkContext context, Expression<Func<TDocument, bool>> filter, FindOptions options = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(filter), filter);

            using (var cursor = await MongoCollection.FindAsync(filter, options: null, cancellationToken: context.CancellationToken))
            {
                return await cursor.ToListAsync();
            }
        }

        public async Task<IEnumerable<TDocument>> Find(IWorkContext context, FilterDefinition<TDocument> filter)
        {
            using (var cursor = await MongoCollection.FindAsync(filter, options: null, cancellationToken: context.CancellationToken))
            {
                return await cursor.ToListAsync();
            }
        }

        public async Task Delete(IWorkContext context, FilterDefinition<TDocument> filter)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(filter), filter);

            var options = new DeleteOptions();

            await MongoCollection.DeleteManyAsync(filter, options, context.CancellationToken);
        }

        public async Task<long> Count(IWorkContext context, FilterDefinition<TDocument> filter = null)
        {
            Verify.IsNotNull(nameof(context), context);

            var filterDefinition = filter ?? FilterDefinition<TDocument>.Empty;

            return await MongoCollection.CountAsync(filterDefinition, options: null, cancellationToken: context.CancellationToken);
        }
    }
}