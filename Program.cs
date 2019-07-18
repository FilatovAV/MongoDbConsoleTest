using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDbConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string con = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            MongoClient client = new MongoClient(con);
            GetDatabaseNames(client).GetAwaiter();
            Console.ReadLine();


            //MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase("local");
            IMongoCollection<BsonDocument> col = database.GetCollection<BsonDocument>("startup_log");
        }
        private static async Task GetDatabaseNames(MongoClient client)
        {
            using (var cursor = await client.ListDatabasesAsync())
            {
                var mdb_database_collection = await cursor.ToListAsync();
                foreach (var mdb_database in mdb_database_collection)
                {
                    //Выводим на экран имя базы данных
                    Console.WriteLine(mdb_database["name"]);

                    //Создаем объект IMongoDatabase чтобы получить коллекции у этой базы данных
                    IMongoDatabase database = client.GetDatabase(mdb_database["name"].ToString());

                    //Обращаемся к объекту IMongoDatabase чтобы получить курсор на входящие в базу данных коллекции
                    using (var mdb_collections_cur = await database.ListCollectionsAsync())
                    {
                        //Получаем коллекцию коллекций
                        var mdb_collections = await mdb_collections_cur.ToListAsync();
                        foreach (var mdb_collection in mdb_collections)
                        {
                            //Получем имя очередной коллекции
                            Console.WriteLine($"- {mdb_collection["name"]}");
                        }
                    }
                }
            }
        }
    }
}
