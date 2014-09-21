﻿using System.Collections.Generic;
using System.Linq;
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
            var contestants = Enumerable.Range(0, 40).Select(e => string.Format("{0}", e)).ToArray();
            var teamNameGenerator = new TeamNameGenerator(Enumerable.Range(0, 200).Select(e => string.Format("Team #{0}", e)).ToArray());
            var locationGenerator = new LocationGenerator(Enumerable.Range(0, 100).Select(e => new Auditory(string.Format("Auditory #{0}", e), 10)).ToArray());
            var roundArrangementGenerator = new RoundArrangementGenerator(contestants, 3);
            var teams = GenerateTeams(roundArrangementGenerator, locationGenerator, teamNameGenerator).ToList();

            var adjacencies = contestants.ToDictionary(c => c, c => new HashSet<string>());
            foreach (var team in teams)
                for (int i = 0; i < team.Contestants.Count; i++)
                {
                    for (int j = i + 1; j < team.Contestants.Count; j++)
                    {
                        var first = string.Compare(team.Contestants[i], team.Contestants[j]) <= 0 ? team.Contestants[i] : team.Contestants[j];
                        var second = string.Compare(team.Contestants[i], team.Contestants[j]) <= 0 ? team.Contestants[j] : team.Contestants[i];

                        if (adjacencies[first].Contains(second))
                            Assert.Fail();
                        adjacencies[first].Add(second);
                    }
                }

            foreach (var grouping in teams.GroupBy(team => team.RoundNumber))
            {
                var actualContestants = grouping.SelectMany(team => team.Contestants).OrderBy(e => e).Distinct().ToArray();
                CollectionAssert.AreEquivalent(contestants, actualContestants);
            }

            Assert.True(teams.Count == RoundsCount * ((contestants.Length + 2) / 3));
        }

        private static IEnumerable<Team> GenerateTeams(RoundArrangementGenerator roundArrangementGenerator, LocationGenerator locationGenerator, TeamNameGenerator teamNameGenerator)
        {
            foreach (var round in roundArrangementGenerator.Generate(RoundsCount))
            {
                locationGenerator.Reset();
                foreach (var team in round.Arrangement.Select(members
                    => new Team(teamNameGenerator.GetNext(), members, locationGenerator.GetNext(), round.Number)))
                    yield return team;
            }
        }
    }
}