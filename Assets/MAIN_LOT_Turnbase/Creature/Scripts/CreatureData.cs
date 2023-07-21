using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public class CreatureData: EntityData
    {
        public CreatureType CreatureType;
        public int CurrentShield;
        public int CurrentExp;
        public int TurnCount;
        public int CurrentDamage;
        public int StregthBoostRemain;

        // Just used for BattleMode
        public JIInventoryItem GetInventoryItem()
        {
            return SavingSystemManager.Instance.GetInventoryItemByName(EntityName);
        }
    }
}