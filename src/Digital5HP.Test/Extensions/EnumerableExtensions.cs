namespace Digital5HP.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        private static readonly Random Rng = new();
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            return list[Rng.Next(list.Count)];
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();

            var n = list.Count;
            while (n > 1) {
                n--;
                var k = Rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }

            return list;
        }
    }
}
