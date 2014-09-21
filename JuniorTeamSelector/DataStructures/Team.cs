using System.Collections.Generic;

namespace JuniorTeamSelector
{
    public class Team
    {
        public Team(string name, List<string> contestants, Location location, int roundNumber)
        {
            Name = name;
            Contestants = contestants;
            Location = location;
            RoundNumber = roundNumber;
        }

        public string Name { get; set; }

        public List<string> Contestants { get; set; }

        public Location Location { get; set; }

        public int RoundNumber { get; set; }
    }
}