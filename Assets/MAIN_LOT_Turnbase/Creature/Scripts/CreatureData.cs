using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [Serializable]
    public class CreatureData: EntityData
    {
        public CreatureType CreatureType;
        public int CurrentShield;
        public int CurrentExp;
        public int TurnCount;
        public int CurrentDamage;
        public List<EffectCache> EffectCaches = new();

        // Just used for BattleMode
        public JIInventoryItem GetInventoryItem()
        {
            return SavingSystemManager.Instance.GetInventoryItemByName(EntityName);
        }
    }

    [Serializable]
    public class EffectCache
    {
        public SkillEffectType EffectType;
        public int EffectRemain;

        public EffectCache(SkillEffectType effectType)
        {
            EffectType = effectType;
        }
    }
}