using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

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
        
        public static List<TilePosition> SpiralPatternConstructor(int tileAmount)
        {
            var listTilePos = new List<TilePosition>();
            var spiralSpace = Mathf.RoundToInt(Mathf.Sqrt(tileAmount)) + 1;

            int printValue = 0;
            int c1 = 0, c2 = 1;
            while (printValue < spiralSpace * spiralSpace)
            {
                //Right Direction Move  
                for (int i = c1 + 1; i <= c2; i++)
                    listTilePos.Add(new TilePosition(c1, i, printValue++));
                //Up Direction Move  
                for (int j = c1 + 1; j <= c2; j++)
                    listTilePos.Add(new TilePosition(j, c2, printValue++));
                //Left Direction Move  
                for (int i = c2 - 1; i >= c1; i--)
                    listTilePos.Add(new TilePosition(c2, i, printValue++));
                //Down Direction Move  
                for (int j = c2 - 1; j >= c1; j--)
                    listTilePos.Add(new TilePosition(j, c1, printValue++));
                c1--;
                c2++;
            }

            return listTilePos;
        }
    }
}