// -----------------------------------------------------------------------------------------------
//  MonsterHelper.cs by Marcus Medina, Copyright (C) 2022, Campus Mölndal.
//  Based on the work from Codic Education AB.
//  Published under GNU General Public License v3 (GPL-3)
// -----------------------------------------------------------------------------------------------

namespace MongoCrud.Facades
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using MongoCrud.Data;
    using MongoCrud.Models;
    using MongoDB.Driver;

    /// <summary>
    /// Facadeklass för monsterhantering
    /// +-----------+        +-----------+       +-----------+      +-----------+
    /// | App       |        | Facade    |       | DbClass   |      | Mongo     |
    /// |           | -----> |           | ----> |           | ---> |           |
    /// +-----------+        +-----------+       +-----------+      +-----------+
    /// Inte nödvändigt men en facade kan ibland förenkla hanteringen av data genom att fylla på
    /// med information där det saknas mm.
    /// Facaden innehåller bara en massa specialmetoder som gör det enklare att använda själva databasen
    /// Facaden anropar i sin tur MongoFacaden. MongoFacaden gör inte mycket mer än att tillföra lite enklare
    /// hantering av databasen. 
    /// </summary>
    public class MonsterHelper
    {
        // Databasinstans
        private readonly MongoDatabase db;
        private readonly MongoFacade help;
        /// <summary>
        /// Monsterhelper constructor
        /// </summary>
        /// <param name="databaseName">Namn på databasen</param>
        /// <param name="collectionName">Namn på collection</param>
        public MonsterHelper(string databaseName = "", string collectionName = "")
        {
            if (collectionName.Length == 0) collectionName = "Monsters"; // default
            db = new MongoDatabase(databaseName, collectionName);
            help = new MongoFacade(databaseName, CollectionName);
        }

        /// <summary>
        /// CollectionNamn
        /// </summary>
        public string CollectionName { get => db.CollectionName; }

        /// <summary>
        /// Databasnamn
        /// </summary>
        public string DatabaseName { get => db.DatabaseName; }

        /// <summary>
        /// Skapar ett monster i databasen
        /// </summary>
        /// <param name="monster">Ett monster</param>
        public void Add(Monster monster) => db.InsertDocument(CollectionName, monster);

        /// <summary>
        /// Lägger till en hel samling monster frå
        /// </summary>
        /// <param name="monsters"></param>
        public void Add(Monster[] monsters) => db.InsertDocuments(CollectionName, monsters);

        /// <summary>
        /// Skapar ett monster, bara om den inte finns redan
        /// </summary>
        /// <param name="monster">Monster</param>
        public void AddIfNotExist(Monster monster)
        {
            var add = FindManyByName(monster.Name)?
                .Where(m =>
                    m.Origin.Place == monster.Origin.Place &&
                    m.Origin.Country == monster.Origin.Country);
            if (add == null)
                Add(monster);
        }

        /// <summary>
        /// Loopar igenom en array av monsters och lägger till de som inte finns
        /// </summary>
        /// <param name="monsters"></param>
        public void AddIfNotExist(Monster[] monsters)
        {
            monsters.ToList().ForEach(monster => AddIfNotExist(monster));
        }

        /// <summary>
        /// Raderar ett monster
        /// </summary>
        /// <param name="monster"></param>
        public void Delete(Monster monster) => db.DeleteDocument<Monster>(CollectionName, monster.Id);

        /// <summary>
        /// Raderar ett monster baserat på Id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(Guid id) => db.DeleteDocument<Monster>(CollectionName, id);

        /// <summary>
        /// Raderar ett monster baserat på namn
        /// </summary>
        /// <param name="name"></param>
        public void Delete(string name)
        {
            var kill = Find(name);
            if (kill != default) db.DeleteDocument<Monster>(CollectionName, kill.Id);
        }

        /// <summary>
        /// Hämtar ett monster baserat på Id
        /// </summary>
        /// <param name="id">Id på monstret</param>
        /// <returns>Ett monster</returns>
        public Monster? Find(Guid id) => help.LoadDocumentById<Monster>(CollectionName, id);

        /// <summary>
        /// Hämtar ett monster baserat på namn
        /// </summary>
        /// <param name="name">Namn på monstret</param>
        /// <returns>Ett monster</returns>
        public Monster? Find(string name) => help.LoadDocumentByName<Monster>(CollectionName, name);

        /// <summary>
        /// Hitta monster baserat på egenskap, exempelvis {"country":"Norway"}
        /// </summary>
        /// <param name="field">Datafält</param>
        /// <param name="data">värde</param>
        /// <returns></returns>
        public List<Monster>? FindMany(string field, string data) => db.LoadAllDocuments<Monster>(CollectionName, field, data);

        /// <summary>
        /// Hitta monster baserat på land
        /// </summary>
        /// <param name="country">Land</param>
        /// <returns>En samling monster från vald land</returns>
        public List<Monster>? FindManyByCountry(string country) => db.LoadAllDocuments<Monster>(CollectionName, "Origin.Country", country);

        /// <summary>
        /// Hitta monster baserat på namn
        /// </summary>
        /// <param name="name">namn</param>
        /// <returns>En samling monster med matchande namn</returns>
        public List<Monster>? FindManyByName(string name) => db.LoadAllDocuments<Monster>(CollectionName, "Name", name);

        /// <summary>
        /// Hitta monster baserat på stad
        /// </summary>
        /// <param name="place">stad</param>
        /// <returns>En samling monster med matchande stad</returns>
        public List<Monster>? FindManyByPlace(string place) => db.LoadAllDocuments<Monster>(CollectionName, "Origin.Place", place);

        /// <summary>
        /// Hämta alla monster
        /// </summary>
        /// <returns>En lista med monster</returns>
        public List<Monster>? GetAll() => db.LoadAllDocuments<Monster>(CollectionName);

        /// <summary>
        /// Uppdaterar ett monster
        /// </summary>
        /// <param name="monster">Monster</param>
        public void Update(Monster monster) => db.UpdateDocument(CollectionName, monster.Id, monster);
    }
}