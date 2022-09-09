// -----------------------------------------------------------------------------------------------
//  Monster.cs by Marcus Medina, Copyright (C) 2022, Campus Mölndal.
//  Based on the work from Codic Education AB.
//  Published under GNU General Public License v3 (GPL-3)
// -----------------------------------------------------------------------------------------------

namespace MongoCrud.Models
{
    using System;

    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Monster definition
    /// </summary>
    public class Monster
    {
        /// <summary>
        /// MonsterId
        /// </summary>
        [BsonId]
        public Guid Id { get; set; }
        /// <summary>
        /// Monsternamn
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// Monster utsprungsland
        /// </summary>
        public Origin Origin { get; set; } = new Origin();
    }
}