using System;
using System.Collections.Generic;
using System.Linq;

namespace JuniorTeamSelector
{
    public class TeamsGenerator
    {
        public static IEnumerable<int[]> Generate(int contestantsCount, int teamMembersCount, int roundsCount)
        {
            var permutation = Enumerable.Range(0, contestantsCount).ToArray();
            var random = new Random();
            var adjacent = permutation.ToDictionary(e => e, e => new HashSet<int>());
            for (int i = 0; i < roundsCount; ++i)
            {
                do
                {
                    permutation = permutation.OrderBy(e => random.Next()).ToArray();
                } while (!Check(permutation, adjacent, teamMembersCount));
                Relax(permutation, adjacent, teamMembersCount);
                yield return permutation;
            }
        }

        private static void Relax(int[] permutation, Dictionary<int, HashSet<int>> adjacent, int teamMembersCount)
        {
            for (int j = 0; j < permutation.Length; j += teamMembersCount)
            {
                for (int k = j; k < permutation.Length && k < j + teamMembersCount; ++k)
                    for (int l = k + 1; l < permutation.Length && l < j + teamMembersCount; ++l)
                    {
                        adjacent[permutation[k]].Add(permutation[l]);
                        adjacent[permutation[l]].Add(permutation[k]);
                    }
            }
        }

        private static bool Check(int[] permutation, Dictionary<int, HashSet<int>> adjacent, int teamMembersCount)
        {
            for (int j = 0; j < permutation.Length; j += teamMembersCount)
            {
                for (int k = j; k < permutation.Length && k < j + teamMembersCount; ++k)
                    for (int l = k + 1; l < permutation.Length && l < j + teamMembersCount; ++l)
                        if (adjacent[permutation[k]].Contains(permutation[l]))
                            return false;
            }
            return true;
        }
    }
}