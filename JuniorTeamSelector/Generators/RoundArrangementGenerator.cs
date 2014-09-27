using System.Collections.Generic;
using System.Linq;
using Core.DataStructures;
using JuniorTeamSelector.Utilities;

namespace JuniorTeamSelector.Generators
{
    public class RoundArrangementGenerator<TContestant>
    {
        private readonly TContestant[] contestants;
        private readonly int teamMembersCount;
        private readonly Dictionary<TContestant, HashSet<TContestant>> pastAdjacencies;
        private readonly HashSet<TContestant> contestantsInIncompleteTeams;

        private int roundNumber = 1;

        public RoundArrangementGenerator(TContestant[] contestants, int teamMembersCount)
        {
            this.contestants = contestants;
            this.teamMembersCount = teamMembersCount;
            this.pastAdjacencies = contestants.ToDictionary(contestant => contestant, contestant => new HashSet<TContestant>());
            this.contestantsInIncompleteTeams = new HashSet<TContestant>();
        }

        public IEnumerable<Round<TContestant>> Generate(int roundsCount)
        {
            while (roundsCount-- > 0)
            {
                yield return GenerateNext();
            }
        }

        private Round<TContestant> GenerateNext()
        {
            return new Round<TContestant>(GenerateArrangement(), roundNumber++);
        }

        private List<List<TContestant>> GenerateArrangement()
        {
            List<List<TContestant>> result;
            while (!TryGenerateArrangement(out result)) ;
            return result;
        }

        private bool TryGenerateArrangement(out List<List<TContestant>> result)
        {
            var currentContestants = RandomHelper.Shuffle(contestants);
            result = new List<List<TContestant>>();
            while (currentContestants.Length > 0)
            {
                List<TContestant> suggestedTeam;
                if (!TrySuggestTeam(currentContestants, out suggestedTeam))
                    return false;
                result.Add(suggestedTeam);
                currentContestants = RandomHelper.Shuffle(currentContestants.Where(e => !suggestedTeam.Contains(e))).ToArray();
            }
            var incompleteTeamContestants = result
                .Where(team => team.Count != teamMembersCount)
                .SelectMany(team => team)
                .ToList();
            if (incompleteTeamContestants.Any(contestantsInIncompleteTeams.Contains))
                return false;
            foreach (var contestant in incompleteTeamContestants)
                contestantsInIncompleteTeams.Add(contestant);

            foreach (var members in result)
                foreach (var member in members)
                    foreach (var otherMember in members)
                        pastAdjacencies[member].Add(otherMember);
            return true;
        }

        private bool TrySuggestTeam(TContestant[] currentContestants, out List<TContestant> suggestedTeam)
        {
            int suggestedTeamSize = currentContestants.Length < teamMembersCount
                ? currentContestants.Length
                : currentContestants.Length < 2 * teamMembersCount
                    ? (currentContestants.Length + 1) / 2
                    : teamMembersCount;

            suggestedTeam = new List<TContestant>();
            foreach (var contestant in currentContestants)
            {
                if (suggestedTeam.Any(other => pastAdjacencies[other].Contains(contestant)))
                    continue;
                suggestedTeam.Add(contestant);
                if (suggestedTeam.Count == suggestedTeamSize)
                    return true;
            }
            return false;
        }
    }
}