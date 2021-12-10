using MongoDB.Driver;
using MongoDB.Bson;

namespace MyNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            IMongoDatabase db = dbClient.GetDatabase("grocery");
            InsertSupplier(db);
            ShowCollection("supplier", db);

        }

        static void ShowCollection(string col_name, IMongoDatabase db)
        {
            var col = db.GetCollection<BsonDocument>(col_name);
            var all_docs = col.Find(new BsonDocument()).Project("{_id:0}").ToList();
            all_docs.ForEach(doc => { Console.WriteLine(doc); });
        }
        static void InsertGoods(IMongoDatabase db)
        {
            Console.WriteLine("Введіть ім'я товару");
            string? name = Console.ReadLine();
            Console.WriteLine("Введіть Кількість товару");
            string? quan = Console.ReadLine();
            var doc = new BsonDocument { { "name", name }, { "quantity", Int64.Parse(quan) } };
            InsertToCollection("goods", db, doc);
        }

        static void InsertReceiptGoods(IMongoDatabase db)
        {
            var goods_col = db.GetCollection<BsonDocument>("goods");
            ShowCollection("goods", db);
            var all_docs = goods_col.Find(new BsonDocument()).ToList();
            Console.WriteLine("ВВедіть \"name\" для якого товару дана поставка");
            string? name;
            ObjectId id = new ObjectId();
            bool flag = true;
            while (flag)
            {
                name = Console.ReadLine();
                all_docs.ForEach(doc =>
                    {
                        if (doc.GetValue("name") == name)
                        {
                            id = ObjectId.Parse(doc.GetValue("_id").ToString());
                            flag = false;
                        }
                    }
                );

            }
            Console.WriteLine("Введіть ціну за одиницю товару");
            string? ppg = Console.ReadLine();
            var doc = new BsonDocument { { "goods", id }, { "date", DateTime.UtcNow }, { "price_per_good", Int64.Parse(ppg) } };
            InsertToCollection("receipt_goods", db, doc);
        }
        static void InsertSells(IMongoDatabase db)
        {
            var goods_col = db.GetCollection<BsonDocument>("goods");
            var all_docs = goods_col.Find(new BsonDocument()).ToList();
            ShowCollection("goods", db);
            Console.WriteLine("ВВедіть \"name\" для якого товару даний продаж");
            string? name;
            ObjectId id = new ObjectId();
            bool flag = true;
            while (flag)
            {
                name = Console.ReadLine();
                all_docs.ForEach(doc =>
                    {
                        if (doc.GetValue("name") == name)
                        {
                            id = ObjectId.Parse(doc.GetValue("_id").ToString());
                            flag = false;
                        }
                    }
                );

            }
            Int64 price_per_good = 0;
            var _col = db.GetCollection<BsonDocument>("receipt_goods");
            var all = _col.Find(new BsonDocument()).ToList();
            all.ForEach(doc =>
            {
                if (doc.GetValue("goods") == id)
                    price_per_good = doc.GetValue("price_per_good").ToInt64();
            });
            Console.WriteLine("Введіть кількісь поданих товарів");
            string? sold = Console.ReadLine();
            var doc = new BsonDocument { { "goods", id }, { "month", DateTime.UtcNow.Month },
                                        {"sold",Int64.Parse(sold) },
                                        {"earned_money",Int64.Parse(sold)*price_per_good} };
            InsertToCollection("sells", db, doc);
        }

        static void InsertSupplier(IMongoDatabase db)
        {
            Console.WriteLine("Введіть ім'я постачальника ");
            string? name = Console.ReadLine();
            Console.WriteLine("Введіть адресу постачальника ");
            string? address = Console.ReadLine();
            Console.WriteLine("Введіть Телефон постачальника ");
            string? tel = Console.ReadLine();
            var doc = new BsonDocument { { "name", name }, { "address", address }, { "telephoe", tel } };
            InsertToCollection("supplier", db, doc);
        }

        static void InsertToCollection(string col_name, IMongoDatabase db, BsonDocument doc)
        {
            var col = db.GetCollection<BsonDocument>(col_name);
            col.InsertOne(doc);
        }
    }
}