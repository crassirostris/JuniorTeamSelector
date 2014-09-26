using System.Collections.Generic;
using System.Linq;

namespace JuniorTeamSelector
{
    //TODO: No contestant goes in non-complete team twice

    public class RoundArrangementGenerator<TContestant>
    {
        private readonly TContestant[] contestants;
        private readonly int teamMembersCount;
        private readonly Dictionary<TContestant, HashSet<TContestant>> pastAdjacencies; 

        private int roundNumber = 1;

        public RoundArrangementGenerator(TContestant[] contestants, int teamMembersCount)
        {
            this.contestants = contestants;
            this.teamMembersCount = teamMembersCount;
            this.pastAdjacencies = contestants.ToDictionary(contestant => contestant, contestant => new HashSet<TContestant>());
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
            foreach (var members in result)
                foreach (var member in members)
                    foreach (var otherMember in members)
                        pastAdjacencies[member].Add(otherMember);
            return true;
        }

        private bool TrySuggestTeam(TContestant[] currentContestants, out List<TContestant> suggestedTeam)
        {
            suggestedTeam = new List<TContestant>();
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