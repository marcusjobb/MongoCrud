// -----------------------------------------------------------------------------------------------
//  MongoDatabase.cs by Marcus Medina, Copyright (C) 2022, Campus Mölndal.
//  Based on the work from Codic Education AB.
//  Published under GNU General Public License v3 (GPL-3)
// -----------------------------------------------------------------------------------------------

namespace MongoCrud.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;

    using MongoDB.Bson;
    using MongoDB.Driver;

    /// <summary>
    /// Ett exempel på hur man kan kommunicera med MongoDB
    /// </summary>
    public class MongoDatabase
    {
        // install-package MongoDB.driver
        private readonly IMongoDatabase db; // Databasen

        /// <summary>
        /// Namn på Collection
        /// </summary>
        public string CollectionName { get; set; }
        /// <summary>
        /// Namn på databasen
        /// </summary>
        public string DatabaseName { get; }

        #region Connect to database

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseName">Databasnamn</param>
        /// <param name="collection">Collectionnamn</param>
        public MongoDatabase(string databaseName = "", string collection = "data")
        {
            if (databaseName.Length == 0)
            {
                databaseName = System.Reflection.Assembly.GetExecutingAssembly()?.FullName?.Split(',')[0] ?? "";
            }
            DatabaseName = databaseName;
            CollectionName = collection;

            //Create new database connection
            var settings = MongoClientSettings.FromConnectionString(GetConnectionString());
            var client = new MongoClient(settings);
            db = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Hämtar Connectionstring från systemvariabel (BookStoreMongoDB) eller localhost
        /// </summary>
        /// <returns></returns>
        private static string GetConnectionString()
        {            
            var connectionString = Environment.GetEnvironmentVariable("BookStoreMongoDB");
            if (string.IsNullOrEmpty(connectionString)) connectionString = "mongodb://localhost";

            return connectionString;
        }

        #endregion Connect to database

        #region Create
        /// <summary>
        /// Lägger till dokument
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="document">Dokument-namn</param>
        public void InsertDocument<T>(string collectionName, T document)
        {
            try
            {
                db.GetCollection<T>(collectionName).InsertOne(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Sparar flera dokument
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        public void InsertDocuments<T>(string collectionName, T[] documents)
        {
            try
            {
                db.GetCollection<T>(collectionName).InsertMany(documents);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion Create

        #region Read
        /// <summary>
        /// Hämtar dokument med filter
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="filter">Sökfilter</param>
        /// <returns></returns>
        internal T? FetchData<T>(string collectionName, FilterDefinition<T> filter)
        {
            try
            {
                return db.GetCollection<T>(collectionName).Find(filter).First();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return default;
        }


        /// <summary>
        /// Läser alla dokument i collection
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <returns>Lista av dokument</returns>
        public List<T>? LoadAllDocuments<T>(string collectionName)
        {
            try
            {
                return db.GetCollection<T>(collectionName).Find(new BsonDocument()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return default;
        }


        /// <summary>
        /// Läser alla dokument i collection
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="field">Sökfält</param>
        /// <param name="data">Data</param>
        /// <returns>Lista av dokument</returns>
        public List<T>? LoadAllDocuments<T>(string collectionName, string field, string data)
        {
            try
            {
                var filter = Builders<T>.Filter.Eq(field, data);
                return db.GetCollection<T>(collectionName).Find(filter).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return default;
        }

        #endregion Read

        #region Update

        /// <summary>
        /// Uppdatera dokument
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="id">Dokument Id</param>
        public void UpdateDocument<T>(string collectionName, Guid id, T document)
        {
            var binData = new BsonBinaryData(id, GuidRepresentation.Standard);
            _ = db.GetCollection<T>(collectionName)
                .ReplaceOne(
                    new BsonDocument("_id", binData),
                    document,
                    new ReplaceOptions { IsUpsert = true });
        }


        #endregion Update

        #region Delete

        /// <summary>
        /// Raderar dokument i given collection med given Id
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="id">Dokument Id</param>
        /// <returns>Dokument</returns>
        public void DeleteDocument<T>(string collectionName, Guid id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            _ = db.GetCollection<T>(collectionName).DeleteOne(filter);
        }

               /// <summary>
        /// Raderar dokument i given collection, med fält som matchar datan
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="field">Sökfält</param>
        /// <param name="data">Data</param>
        public void DeleteDocuments<T>(string collectionName, string field, string data)
        {
            var filter = Builders<T>.Filter.Eq(field, data);
            _ = db.GetCollection<T>(collectionName).DeleteMany(filter);
        }


        /// <summary>
        /// Raderar en collection med givet namn
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        public void DropCollection<T>(string collectionName)
        {
            var indexes = db.GetCollection<T>(collectionName).Indexes.List().ToList();

            db.DropCollection(collectionName);
        }

        #endregion Delete

        #region Index

        /// <summary>
        /// Skapar index
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="field">Sökfält</param>
        public void CreateIndex<T>(string collectionName, Expression<Func<T, object>> field)
        {
            var indexOptions = new CreateIndexOptions();
            var indexKeys = Builders<T>.IndexKeys.Ascending(field);
            var indexModel = new CreateIndexModel<T>(indexKeys, indexOptions);
            try
            {
                db.GetCollection<T>(collectionName).Indexes.CreateOne(indexModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Radera ett index
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="name">Namn</param>
        public void DropIndex<T>(string collectionName, string name)
        {
            try
            {
                db.GetCollection<T>(collectionName).Indexes.DropOne(name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion Index
    }
}