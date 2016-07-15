using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace MongoDBTester
{
    class MongoDBHelper
    {
        private MongoClient db;
        private bool connected = false;
        private IMongoDatabase mDB;

        public MongoDBHelper()
        {
            this.db = new MongoClient("mongodb://localhost/?safe=true");
            connected = true;

        }

        public bool IsConnected
        {
            get { return this.connected; }
        }

        public async Task<List<string>> Databases()
        {
            List<string> dbs = new List<string>();
            List<BsonDocument> bDocs = new List<BsonDocument>();
            if (this.connected)
            {

                using (var cursor = await db.ListDatabasesAsync())
                {
                    await cursor.ForEachAsync(d => bDocs.Add(d));
                }

                foreach (var bDoc in bDocs)
                {
                    //BsonElement bEle = new BsonElement();
                    //bEle.Name

                    var bElements = from elements in bDoc.Elements
                                    where elements.Name == "name"
                                    select elements;

                    if (bElements != null && bElements.Count() > 0)
                    {
                        dbs.Add(bElements.First().Value.AsString);
                    }
                }

                /*
                List<BsonDocument>b = this.db.ListDatabases().ToList();
                foreach (var u in b)
                {
                    dbs.Add(u.ToString());
                }
                */
            }
            return dbs;
        }

        public void SetDB(string dbName)
        {
            if (this.db != null && this.connected)
            {
                mDB = this.db.GetDatabase(dbName);
            }
        }
        public async Task<List<string>> Search(string collectionName)
        {
            List<string> items = new List<string>();
            if (this.db != null && this.connected && mDB != null)
            {
                var collection = this.mDB.GetCollection<BsonDocument>(collectionName);
                var filter = new BsonDocument();
                var count = 0;
                using (var cursor = await collection.FindAsync(filter))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        var batch = cursor.Current;
                        foreach (var document in batch)
                        {
                            items.Add(document["title"].AsString);
                            // process document
                            count++;
                        }
                    }
                }

                /*
                foreach (var t in collection.AsQueryable<Links>().Select(e => e.title).Distinct())
                {
                    

                }
                
                var iList = collection.AsQueryable().Select(e => e.Id.ToString()).Distinct();

                foreach (var iItem in iList)
                {
                    items.Add(iItem);
                }
                */
            }
            return items;
        }

        public class Links
        {
            public ObjectId Id { get; set; }
            public string title { get; set; }
        }
    }
}
