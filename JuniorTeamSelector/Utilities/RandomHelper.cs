using System;
using System.Collections.Generic;
using System.Linq;

namespace JuniorTeamSelector
{
    public static class RandomHelper
    {
        private static readonly Random random = new Random();

        public static T[] Shuffle<T>(T[] collection)
        {
            return collection.OrderBy(t => random.Next()).ToArray();
        }

        public static List<T> Shuffle<T>(List<T> collection)
        {
            return collection.OrderBy(t => random.Next()).ToList();
        }

        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> collection)
        {
            return collection.OrderBy(t => random.Next());
        }
    }
}