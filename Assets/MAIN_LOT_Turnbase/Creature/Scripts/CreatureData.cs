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
        public List<EffectCache> EffectCaches;

        public CreatureData() { }
        
        public CreatureData(CreatureData creatureData)
        {
            EntityName = creatureData.EntityName;
            SkinAddress = creatureData.SkinAddress;
            Position = creatureData.Position;
            Rotation = creatureData.Rotation;
            FactionType = creatureData.FactionType;
            CreatureType = creatureData.CreatureType;
            CurrentShield = creatureData.CurrentShield;
            CurrentExp = creatureData.CurrentExp;
            TurnCount = creatureData.TurnCount;
            CurrentDamage = creatureData.CurrentDamage;
            foreach (var effectCache in creatureData.EffectCaches)
                EffectCaches.Add(new EffectCache(effectCache));
        }

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

        public EffectCache(EffectCache effectCache)
        {
            EffectType = effectCache.EffectType;
            EffectRemain = effectCache.EffectRemain;
        }
    }
}