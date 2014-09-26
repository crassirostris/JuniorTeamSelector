using System.Collections.Generic;

namespace JuniorTeamSelector
{
    public class Team
    {
        public Team(TeamCredentials credentials, List<Contestant> contestants, Location location, int roundNumber)
        {
            Credentials = credentials;
            Contestants = contestants;
            Location = location;
            RoundNumber = roundNumber;
        }

        public TeamCredentials Credentials { get; set; }

        public List<Contestant> Contestants { get; set; }

        public Location Location { get; set; }

        public int RoundNumber { get; set; }
    }
}