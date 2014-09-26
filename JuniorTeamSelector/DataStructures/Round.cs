using System.Collections.Generic;

namespace JuniorTeamSelector
{
    public class Round<TContestant>
    {
        public Round(List<List<TContestant>> arrangement, int number)
        {
            Arrangement = arrangement;
            Number = number;
        }

        public List<List<TContestant>> Arrangement { get; set; }

        public int Number { get; set; }
    }
}