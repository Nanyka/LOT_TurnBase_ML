using System.Collections.Generic;

namespace JumpeeIsland
{
    [System.Serializable]
    public class CreatureStats
    {
        public int HealthPoint;
        public int Strength;
        public int Armor;
        public int CostToLevelUp;
        public int ExpReward;
        public List<CommandName> Commands;
        public CreatureType CreatureType;
    }
}