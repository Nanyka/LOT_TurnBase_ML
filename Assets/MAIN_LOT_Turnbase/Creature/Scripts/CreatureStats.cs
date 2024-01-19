using System.Collections.Generic;

namespace JumpeeIsland
{
    [System.Serializable]
    public class CreatureStats
    {
        public int HealthPoint;
        public int Strength;
        public int Armor;
        public int CostToLevelUp; // This param is used as required amount of resource to transform the zombie
        public int ExpReward;
        public float MovingSpeed;
        public List<CommandName> Commands;
        public CreatureType CreatureType;
    }
}