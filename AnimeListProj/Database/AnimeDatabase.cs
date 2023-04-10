using Amazon.Runtime.Documents;
using AnimeList.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeList
{
    internal static class AnimeDatabase
    {
        private static string connectionString;
        private static MongoClient client;
        private static IMongoDatabase database;
        private static IMongoCollection<BsonDocument> collection;

        static AnimeDatabase()
        {
            connectionString = "mongodb://localhost:27017";
            client = new MongoClient(connectionString);
            database = client.GetDatabase("animeList");
            collection = database.GetCollection<BsonDocument>("anime");
        }

        public static List<BsonDocument> GetAnime()
        {
            var result = collection.Find(new BsonDocument()).ToList();

            return result;
        }

        public static void AddAnime(Anime anime)
        {
            var bsonDoc = new BsonDocument
            {
                { "Picture", $"{anime.Picture}" },
                { "Title", $"{anime.Title}" }
            };

            collection.InsertOne(bsonDoc);
        }

        public static void DeleteAnime(Anime anime)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(anime.Id));

            var result = collection.DeleteOne(filter);
        }

        public static void UpdateAnimePicture(Anime anime)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(anime.Id));

            var update = Builders<BsonDocument>.Update.Set("Picture", anime.Picture);

            var result = collection.UpdateOne(filter, update);
        }

        public static void UpdateAnimeTitle(Anime anime)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(anime.Id));

            var update = Builders<BsonDocument>.Update.Set("Title", anime.Title);

            var result = collection.UpdateOne(filter, update);
        }

        public static string GetLastAddedAnimeId()
        {
            var lastDocument = collection.AsQueryable().Last();

            string id = lastDocument.GetValue("_last").ToString().Split(':')[1].Split('"')[1].Split('\\')[0];

            return id;
        }
    }
}
