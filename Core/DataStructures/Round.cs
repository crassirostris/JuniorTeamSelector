using System.Collections.Generic;

namespace Core.DataStructures
{
    public class Round<TContestant>
    {
        public List<List<TContestant>> Arrangement { get; set; }

        public int Number { get; set; }

        public Round(List<List<TContestant>> arrangement, int number)
        {
            Arrangement = arrangement;
            Number = number;
        }
    }
}