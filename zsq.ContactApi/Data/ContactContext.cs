using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using zsq.ContactApi.Config;
using zsq.ContactApi.Models;

namespace zsq.ContactApi.Data
{
    public class ContactContext
    {
        private IMongoDatabase _mongoDatabase;
        //private IMongoCollection<ContactBook> _mongoCollection;
        private MongoConnectionConfig _connectionConfig;

        public ContactContext()
        {

        }

        public ContactContext(IOptionsSnapshot<MongoConnectionConfig> settings)
        {
            _connectionConfig = settings.Value;
            var client = new MongoClient(_connectionConfig.ConnectionString);
            if (client != null)
            {
                _mongoDatabase = client.GetDatabase(_connectionConfig.Database);
            }
        }

        private void CheckAndCreateConnectiion(string collectionName)
        {
            //var collectionList = _mongoDatabase.ListCollections().ToList();
            //var collectionNames = new List<string>();
            var collectionNames = _mongoDatabase.ListCollectionNames().ToList();
            if (!collectionNames.Contains(collectionName))
            {
                _mongoDatabase.CreateCollection(collectionName);
            }
        }

        public IMongoCollection<ContactBook> ContactBooks
        {
            get
            {
                CheckAndCreateConnectiion("ContactBooks");
                return _mongoDatabase.GetCollection<ContactBook>("ContactBooks");
            }
        }

        public IMongoCollection<ContactApplyRequest> ContactApplyRequests
        {
            get
            {
                CheckAndCreateConnectiion("ContactApplyRequests");
                return _mongoDatabase.GetCollection<ContactApplyRequest>("ContactApplyRequests");
            }
        }
    }
}
