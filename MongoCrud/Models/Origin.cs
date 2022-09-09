// -----------------------------------------------------------------------------------------------
//  Origin.cs by Marcus Medina, Copyright (C) 2022, Campus Mölndal.
//  Based on the work from Codic Education AB.
//  Published under GNU General Public License v3 (GPL-3)
// -----------------------------------------------------------------------------------------------
namespace MongoCrud.Models
{
    /// <summary>
    /// Birthplace for the monster
    /// </summary>
    public class Origin
    {
        /// <summary>
        /// Place of birth of the monster
        /// </summary>
        public string Place { get; set; } = "";
        /// <summary>
        /// Country of birth of the monster
        /// </summary>
        public string Country { get; set; } = "";
    }
}