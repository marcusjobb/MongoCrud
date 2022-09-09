// -----------------------------------------------------------------------------------------------
//  MonsterHelper.cs by Marcus Medina, Copyright (C) 2022, Campus Mölndal.
//  Based on the work from Codic Education AB.
//  Published under GNU General Public License v3 (GPL-3)
// -----------------------------------------------------------------------------------------------

namespace MongoCrud.Facades
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MongoCrud.Data;
    using MongoDB.Driver;

    /// <summary>
    /// MongoFacaden gör inte mycket mer än att tillföra lite enklare
    /// hantering av databasen. Man kan lika gärna ta hela koden från den 
    /// och lägga det i MongoDatabase, jag flyttade dock ut metoderna för
    /// att försöka hålla MongoDatabase klassen så ren som möjligt med bara
    /// standardanrop.
    /// </summary>
    public class MongoFacade
    {
        public MongoFacade(string databaseName = "", string collectionName = "")
        {
            if (collectionName.Length == 0) collectionName = "Monsters"; // default
            db = new MongoDatabase(databaseName, collectionName);
        }

        /// <summary>
        /// CollectionNamn
        /// </summary>
        public string CollectionName { get => db.CollectionName; }

        /// <summary>
        /// Databasnamn
        /// </summary>
        public string DatabaseName { get => db.DatabaseName; }


        #region Shortcuts
        /// <summary>
        /// Sparar dokument
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="document">Dokument</param>
        public void InsertDocument<T>(T document) => db.InsertDocument<T>(CollectionName, document);

        /// <summary>
        /// Lägger till flera dokument
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="documents">Dokumenter</param>
        public void InsertDocuments<T>(T[] documents) => db.InsertDocuments<T>(CollectionName, documents);

        /// <summary>
        /// Hämtar specifik dokument genom sökning av "Id" fält
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="id">Dokument Id</param>
        /// <returns>Dokument</returns>
        public T? LoadDocumentById<T>(string collectionName, Guid id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            return db.FetchData(collectionName, filter);
        }

        /// <summary>
        /// Hämtar ett dokument där fältet "Name" har givet värde
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="name"></param>
        /// <returns>Dokument</returns>
        public T? LoadDocumentByName<T>(string collectionName, string name)
        {
            var filter = Builders<T>.Filter.Eq("Name", name);
            return db.FetchData(collectionName, filter);
        }

        /// <summary>
        /// Läser alla dokument i collection
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <returns>Lista av dokument</returns>
        public List<T>? LoadAllDocuments<T>() => db.LoadAllDocuments<T>(CollectionName);


        /// <summary>
        /// Läser in alla dokument i collection
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="field">Sökfält</param>
        /// <param name="data">Data</param>
        /// <returns>Lista av dokument</returns>
        public List<T>? LoadAllDocuments<T>(string field, string data) => db.LoadAllDocuments<T>(CollectionName, field, data);

        /// <summary>
        /// Uppdaterar dokument med given Id
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="id">Dokument Id</param>
        public void UpdateDocument<T>(Guid id, T document) => db.UpdateDocument(CollectionName, id, document);

        /// <summary>
        /// Raderar dokument med given Id
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="id">Dokument Id</param>
        public void DeleteDocument<T>(Guid id) => db.DeleteDocument<T>(CollectionName, id);

        /// <summary>
        /// Radera dokument som matchar given fält och data
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="field">Sökfält</param>
        /// <param name="data">Data</param>
        public void DeleteDocuments<T>(string field, string data) => db.DeleteDocuments<T>(CollectionName, field, data);


        /// <summary>
        /// Söker dokument med specifik fält och specifik värde
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="field">Sökfält</param>
        /// <param name="data">Data</param>
        /// <returns>Dokument</returns>
        public T? LoadDocumentByAny<T>(string field, string data) => LoadDocumentByAny<T>(CollectionName, field, data);

        /// <summary>
        /// Söker dokument med specifik fält och specifik värde
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="collectionName">Collection-namn</param>
        /// <param name="field">Sökfält</param>
        /// <param name="data">Data</param>
        /// <returns>Dokument</returns>
        public T? LoadDocumentByAny<T>(string collectionName, string field, string data)
        {
            var filter = Builders<T>.Filter.Eq(field, data);
            return db.FetchData(collectionName, filter);
        }

        /// <summary>
        /// Hämtar ett dokument där namnet har givet värde
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="name"></param>
        /// <returns>Dokument</returns>
        public T? LoadDocumentByName<T>(string name) => LoadDocumentByName<T>(CollectionName, name);


        /// <summary>
        /// Hämtar specifik dokument via Id fält
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="id">Dokument Id</param>
        /// <returns>Dokument</returns>
        public T? LoadDocumentById<T>(Guid id) => LoadDocumentById<T>(CollectionName, id);



        /// <summary>
        /// Skapar index
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="field">Sökfält</param>
        public void CreateIndex<T>(Expression<Func<T, object>> field) => db.CreateIndex<T>(CollectionName, field);


        /// <summary>
        /// Radera Index
        /// </summary>
        /// <typeparam name="T">Generisk typ</typeparam>
        /// <param name="name">Namn</param>
        public void DropIndex<T>(string name) => db.DropIndex<T>(CollectionName, name);

        #endregion

        // Databasinstans
        private readonly MongoDatabase db;
    }
}