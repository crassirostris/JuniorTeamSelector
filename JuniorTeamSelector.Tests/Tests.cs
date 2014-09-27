using System.Collections.Generic;
using System.Linq;
using Core.DataStructures;
using JuniorTeamSelector.Generators;
using NUnit.Framework;

namespace JuniorTeamSelector.Tests
{
    [TestFixture]
    internal class Tests
    {
        private const int RoundsCount = 8;

        [Test]
        public void GenerateTeams_Test()
        {
            var contestants = Enumerable.Range(0, 40).Select(e => new Contestant(string.Format("{0}", e), string.Format("{0}", e))).ToArray();
            var teamNameGenerator = new TeamCredentialsGenerator(Enumerable.Range(0, 200).Select(e =>
                new TeamCredentials(string.Format("Team #{0}", e), string.Format("Team #{0}", e), e / 25 + 1)).ToArray());
            var locationGenerator = new LocationGenerator(Enumerable.Range(0, 100).Select(e => new Auditory(string.Format("Auditory #{0}", e), 10)).ToArray());
            var roundArrangementGenerator = new RoundArrangementGenerator<Contestant>(contestants, 3);
            var teams = GenerateTeams(roundArrangementGenerator, locationGenerator, teamNameGenerator).ToList();

            var adjacencies = contestants.ToDictionary(c => c, c => new HashSet<Contestant>());
            foreach (var team in teams)
                for (int i = 0; i < team.Contestants.Count; i++)
                {
                    for (int j = i + 1; j < team.Contestants.Count; j++)
                    {
                        var first = string.Compare(team.Contestants[i].Name, team.Contestants[j].Name) <= 0 ? team.Contestants[i] : team.Contestants[j];
                        var second = string.Compare(team.Contestants[i].Name, team.Contestants[j].Name) <= 0 ? team.Contestants[j] : team.Contestants[i];

                        if (adjacencies[first].Contains(second))
                            Assert.Fail();
                        adjacencies[first].Add(second);
                    }
                }

            foreach (var grouping in teams.GroupBy(team => team.RoundNumber))
            {
                var actualContestants = grouping.SelectMany(team => team.Contestants).OrderBy(e => e.Name).Distinct().ToArray();
                CollectionAssert.AreEquivalent(contestants, actualContestants);
            }

            Assert.True(teams.Count == RoundsCount * ((contestants.Length + 2) / 3));

            var config = JuniorTeamSelectorConfig.Instance;

            var incompleteTeams = teams.Where(team => team.Contestants.Count != config.TeamMembersCount).ToList();
            CollectionAssert.AreEquivalent(incompleteTeams.SelectMany(t => t.Contestants), incompleteTeams.SelectMany(t => t.Contestants).Distinct());
        }

        [Test]
        [TestCase(10)]
        public void GenerateTeams_MultipleTimes_Test(int roundsCount)
        {
            for (int i = 0; i < roundsCount; i++)
                GenerateTeams_Test();
        }

        private static IEnumerable<Team> GenerateTeams(RoundArrangementGenerator<Contestant> roundArrangementGenerator, LocationGenerator locationGenerator, TeamCredentialsGenerator teamCredentialsGenerator)
        {
            foreach (var round in roundArrangementGenerator.Generate(RoundsCount))
            {
                locationGenerator.Reset();
                foreach (var team in round.Arrangement.Select(members
                    => new Team(teamCredentialsGenerator.GetNext(round.Number), members, locationGenerator.GetNext(), round.Number)))
                    yield return team;
            }
        }
    }
}
