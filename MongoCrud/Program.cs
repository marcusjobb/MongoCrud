// -----------------------------------------------------------------------------------------------
//  Program.cs by Marcus Medina, Copyright (C) 2022, Campus Mölndal.
//  Based on the work from Codic Education AB.
//  Published under GNU General Public License v3 (GPL-3)
// -----------------------------------------------------------------------------------------------
namespace MongoCrud
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MongoCrud.Data;
    using MongoCrud.Facades;
    using MongoCrud.Models;

    internal static class Program
    {
        private static void Main()
        {
            // Öppna databas
            var db = new MongoDatabase("Monsterbash", "Monsters");
            var help = new MongoFacade("Monsterbash", "Monsters");
            // Ta bort gammal index
            db.DropIndex<Monster>("Monsters", "Origin.Country_1");
            // Skapa index
            help.CreateIndex<Monster>(m => m.Name);
            // Mer index
            help.CreateIndex<Monster>(m => m.Origin.Place);
            help.CreateIndex<Monster>(m => m.Origin.Country);

            // Kolla om det finns monster i databasen, annars skapa en bunt
            if (help.LoadAllDocuments<Monster>()?.ToList().Count == 0)
            {
                help.InsertDocuments
                (
                    new Monster[]
                    {
                    new Monster { Name = "Dracula", Origin = new Origin { Place = "Transylvania", Country = "Romania" } },
                    new Monster { Name = "Dracula Jr", Origin = new Origin { Place = "Pennsylvania", Country = "USA" } },
                    new Monster { Name = "Frankenstein", Origin = new Origin { Place = "Geneva", Country = "Switzerland" } },
                    new Monster { Name = "Invisible man", Origin = new Origin { Place = "Iping", Country = "England" } },
                    new Monster { Name = "King Kong", Origin = new Origin { Place = "Skull Island", Country = "Sumatra" } },
                    new Monster { Name = "Mummy", Origin = new Origin { Place = "Hamunaptra", Country = "Egypt" } },
                    new Monster { Name = "Creature from Black lagoon", Origin = new Origin { Place = "Amazonas", Country = "Brazil" } },
                    new Monster { Name = "Chupacabra", Origin = new Origin { Place = "Mexico Place", Country = "Mexico" } },
                    }
                );
            }

            // Sök efter Chupacabras och radera denne
            var chupa = help.LoadDocumentByName<Monster>("Chupacabras");
            if (chupa != default)
            {
                chupa.Name = "Chupacabras";
                //db.UpdateDocument<Monster>(chupa.Id,chupa);
                help.DeleteDocument<Monster>(chupa.Id);
            }

            // Hämta monster födda i USA
            var usa = help.LoadDocumentByAny<Monster>("Origin.Country", "USA");
            PrintMonster(usa!); // null forgiving
            Console.WriteLine();

            // Hämta mumier
            var mummy = help.LoadDocumentByAny<Monster>("Name", "Mummy");
            PrintMonster(mummy!); // null forgiving
            Console.WriteLine();

            // Hämta alla monster
            var allMonster = help.LoadAllDocuments<Monster>()?.OrderBy(m => m.Origin.Country).ThenBy(m => m.Name).ToList();
            PrintAllMonsters(allMonster!); // Null forgiving

            PrintAllMonsters(allMonster!.Where(m => m.Origin.Country.StartsWith("E")).ToList());
        }

        /// <summary>
        /// Skriver ut en lista på monster
        /// </summary>
        /// <param name="allMonster">En lista av monster</param>
        private static void PrintAllMonsters(List<Monster> allMonster)
        {
            allMonster.ForEach(monster => PrintMonster(monster));
            Console.WriteLine();
        }

        /// <summary>
        /// Skriver ut monsterdata
        /// </summary>
        /// <param name="monster">Ett specifikt monster</param>
        private static void PrintMonster(Monster monster)
        {
            Console.WriteLine($"Monster: {monster.Name} ({monster.Id})");
            Console.WriteLine($"Origin : {monster.Origin.Place}, {monster.Origin.Country}");
        }
    }
}