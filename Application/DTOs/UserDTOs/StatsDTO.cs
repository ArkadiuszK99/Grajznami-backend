using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StatsDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int TotalPlayedMatches { get; set; }
        public int MatchesPlayedInLastMonth { get; set; }
        public int MostMatchesInSport { get; set; }
        public string SportWithMostMatches { get; set; }
        public string FavouriteSport { get; set; }
        public int FavouriteSportMatches { get; set; }
        public string Skill { get; set; }
        public string City { get; set; }
        public int NumberOfGamesOrganised { get; set; }
    }
}
