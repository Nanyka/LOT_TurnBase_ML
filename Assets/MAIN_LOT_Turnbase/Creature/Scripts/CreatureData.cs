using UnityEngine;

namespace JumpeeIsland
{
    [System.Serializable]
    public class CreatureData: EntityData
    {
        public int CurrentShield;
        public int CurrentExp;
        public int TurnCount;
        public int CurrentDamage;

        // Just used for BattleMode
        public JIInventoryItem GetInventoryItem()
        {
            return SavingSystemManager.Instance.GetInventoryItemByName(EntityName);
        }
    }
}