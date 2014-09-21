using System;

namespace JuniorTeamSelector
{
    public class TeamNameGenerator
    {
        private readonly string[] teamNames;

        private int teamNamesIndex;

        public TeamNameGenerator(string[] teamNames)
        {
            this.teamNames = RandomHelper.Shuffle(teamNames);
        }

        public string GetNext()
        {
            return teamNames[teamNamesIndex++];
        }
    }
}