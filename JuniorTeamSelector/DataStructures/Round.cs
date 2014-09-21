using System.Collections.Generic;

namespace JuniorTeamSelector
{
    public class Round
    {
        public Round(List<List<string>> arrangement, int number)
        {
            Arrangement = arrangement;
            Number = number;
        }

        public List<List<string>> Arrangement { get; set; }

        public int Number { get; set; }
    }
}