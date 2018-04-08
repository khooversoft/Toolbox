//using Khooversoft.Toolbox;
//using System.Threading.Tasks;

//namespace Khooversoft.MongoDb
//{
//    public class DbCollectionManagement
//    {
//        readonly KeyedDictionary<string, DbCollection> _collection;

//        public DbCollectionManagement(MongoDbContext dbContext)
//        {
//            Verify.IsNotNull(nameof(dbContext), dbContext);

//            _collection = new KeyedDictionary<string, DbCollection>(x => x.CollectionName, x => { x.Parent = this; return x; });

//            DbContext = dbContext;
//        }

//        public MongoDbContext DbContext { get; private set; }

//        /// <summary>
//        /// Collections
//        /// </summary>
//        public KeyedDictionary<string, DbCollection> Collections { get { return _collection; } }

//        /// <summary>
//        /// Build indexes
//        /// </summary>
//        /// <returns>Done</returns>
//        public async Task BuildAllIndexes()
//        {
//            foreach (var item in Collections.Values)
//            {
//                await item.BuildIndexes();
//            }
//        }

//        /// <summary>
//        /// Drop all collections
//        /// </summary>
//        /// <returns>Done</returns>
//        public async Task DropAllCollections()
//        {
//            foreach (var item in Collections.Values)
//            {
//                await item.DropCollection();
//            }
//        }
//    }
//}
