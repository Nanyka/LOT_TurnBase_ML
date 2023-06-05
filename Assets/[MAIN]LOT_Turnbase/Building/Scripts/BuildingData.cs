using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public class BuildingData: EntityData
    {
        public int CurrentShield;
        public int CurrentExp;
        public int CurrentStorage;
        public int TurnCount;
    }
}