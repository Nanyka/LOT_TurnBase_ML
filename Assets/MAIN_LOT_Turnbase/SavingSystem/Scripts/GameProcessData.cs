using System.Collections.Generic;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [System.Serializable]
    public class GameProcessData
    {
        public string currentTutorial;
        public long timestamp;
        public long lastTimestamp;
        public long moveTimestamp;
        public int battleCount;
        public int win1StarCount;
        public int win2StarCount;
        public int win3StarCount;
        public int winStack;
        public int bossUnlock;
        public int score;
    }
}