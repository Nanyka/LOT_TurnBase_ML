using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class MannequinEntity : Entity, ICreatureInit
    {
        private IAnimateComp m_AnimateComp;
        private ISkinComp m_SkinComp;
        
        private CreatureData m_CreatureData;
        private List<CreatureStats> m_CreatureStats;
        private CreatureStats m_CurrentStat;

        private void Awake()
        {
            m_AnimateComp = GetComponent<IAnimateComp>();
            m_SkinComp = GetComponent<ISkinComp>();
        }

        public void Init(CreatureData creatureData)
        {
            m_CreatureData = creatureData;
            m_Transform.position = m_CreatureData.Position;

            RefreshEntity();
        }
        
        public override void Relocate(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            throw new System.NotImplementedException();
        }

        public override EntityData GetData()
        {
            throw new System.NotImplementedException();
        }

        public override FactionType GetFaction()
        {
            throw new System.NotImplementedException();
        }

        public override ISkinComp GetSkin()
        {
            throw new System.NotImplementedException();
        }
        
        private void RefreshEntity()
        {
            // Set stats based on currentLevel
            var creatureLevel = SavingSystemManager.Instance.GetInventoryLevel(m_CreatureData.EntityName);
            m_CreatureData.CurrentLevel = creatureLevel == 0 ? m_CreatureData.CurrentLevel : creatureLevel;
            var inventory = SavingSystemManager.Instance.GetInventoryItemByName(m_CreatureData.EntityName);
            m_CreatureStats = inventory.creatureStats;
            m_CurrentStat = m_CreatureStats[m_CreatureData.CurrentLevel];

            // Initiate entity data if it's new
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_CreatureData.EntityName);
            m_CreatureData.SkinAddress =
                inventoryItem.skinAddress[
                    Mathf.Clamp(m_CreatureData.CurrentLevel, 0, inventoryItem.skinAddress.Count - 1)];
            m_CreatureData.CreatureType = m_CurrentStat.CreatureType;

            // Retrieve entity data
            m_SkinComp.Init(m_CreatureData.SkinAddress, m_AnimateComp);
        }
    }
}