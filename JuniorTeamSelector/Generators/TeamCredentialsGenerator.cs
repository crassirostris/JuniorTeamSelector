using System;
using System.Collections.Generic;
using System.Linq;

namespace JuniorTeamSelector
{
    public class TeamCredentialsGenerator
    {
        private readonly Dictionary<int, List<TeamCredentials>> teamsCredentials;
        private readonly Dictionary<int, int> indices;

        public TeamCredentialsGenerator(TeamCredentials[] teamsCredentials)
        {
            this.teamsCredentials = teamsCredentials
                .GroupBy(tc => tc.Round)
                .ToDictionary(g => g.Key, g => RandomHelper.Shuffle(g).ToList());
            this.indices = this.teamsCredentials.ToDictionary(kvp => kvp.Key, kvp => 0);
        }

        public TeamCredentials GetNext(int roundNumber)
        {
            return teamsCredentials[roundNumber][indices[roundNumber]++];
        }
    }
}