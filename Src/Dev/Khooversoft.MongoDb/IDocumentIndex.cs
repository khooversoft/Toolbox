using Khooversoft.MongoDb.Models.V1;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public interface IDocumentIndex<TDocument>
    {
        DocumentCollection<TDocument> Parent { get; }

        Task<IEnumerable<IndexDetailV1>> ListIndexes(IWorkContext context);

        Task<IndexDetailV1> GetIndexDetail(IWorkContext context, string indexName);

        Task<bool> IndexExist(IWorkContext context, string indexName);

        Task CreateIndex(IWorkContext context, CollectionIndex collectionIndex);

        Task DropIndex(IWorkContext context, string indexName);
    }
}
