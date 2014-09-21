using System.Collections.Generic;
using System.Linq;

namespace JuniorTeamSelector
{
    public class RoundArrangementGenerator
    {
        private readonly string[] contestants;
        private readonly int teamMembersCount;
        private readonly Dictionary<string, HashSet<string>> pastAdjacencies; 

        private int roundNumber = 1;

        public RoundArrangementGenerator(string[] contestants, int teamMembersCount)
        {
            this.contestants = contestants;
            this.teamMembersCount = teamMembersCount;
            this.pastAdjacencies = contestants.ToDictionary(contestant => contestant, contestant => new HashSet<string>());
        }

        public IEnumerable<Round> Generate(int roundsCount)
        {
            while (roundsCount-- > 0)
            {
                yield return GenerateNext();
            }
        }

        private Round GenerateNext()
        {
            return new Round(GenerateArrangement(), roundNumber++);
        }

        private List<List<string>> GenerateArrangement()
        {
            List<List<string>> result;
            while (!TryGenerateArrangement(out result)) ;
            return result;
        }

        private bool TryGenerateArrangement(out List<List<string>> result)
        {
            var currentContestants = RandomHelper.Shuffle(contestants);
            result = new List<List<string>>();
            while (currentContestants.Length > 0)
            {
                List<string> suggestedTeam;
                if (!TrySuggestTeam(currentContestants, out suggestedTeam))
                    return false;
                result.Add(suggestedTeam);
                currentContestants = RandomHelper.Shuffle(currentContestants.Where(e => !suggestedTeam.Contains(e))).ToArray();
            }
            foreach (var members in result)
                foreach (var member in members)
                    foreach (var otherMember in members)
                        pastAdjacencies[member].Add(otherMember);
            return true;
        }

        private bool TrySuggestTeam(string[] currentContestants, out List<string> suggestedTeam)
        {
            suggestedTeam = new List<string>();
            foreach (var contestant in currentContestants)
            {
                if (suggestedTeam.Any(other => pastAdjacencies[other].Contains(contestant)))
                    continue;
                suggestedTeam.Add(contestant);
                if (suggestedTeam.Count == teamMembersCount || suggestedTeam.Count == currentContestants.Length)
                    return true;
            }
            return false;
        }
    }
}