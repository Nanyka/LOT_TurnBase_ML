using System;
using System.Collections.Generic;

namespace JumpeeIsland
{
    public class GeneralAlgorithm
    {
        // The Fisher-Yates shuffle algorithm
        public static void Shuffle<T>(List<T> list)
        {
            var random = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}