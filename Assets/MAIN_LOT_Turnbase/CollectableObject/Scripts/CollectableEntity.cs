using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectableEntity : Entity
    {
        [SerializeField] private CollectableStats[] m_CollectableStats;
        [SerializeField] private CollectComp m_CollectComp;
        
        private CollectableData m_CollectableData;
        private CollectableStats m_CurrentStat;
        
        public void Init(CollectableData collectableData)
        {
            m_CollectableData = collectableData;
            RefreshEntity();
        }
        
        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            throw new System.NotImplementedException();
        }

        public override EntityData GetData()
        {
            return m_CollectableData;
        }
        
        public void DurationDeduct()
        {
            if (m_CurrentStat.IsLongLasting)
                return;

            m_CollectableData.AccumulatedStep++;
            if (m_CollectableData.AccumulatedStep >= m_CurrentStat.MaxTurnToDestroy)
                OnUnitDie.Invoke(this);
        }

        public override CommandName GetCommand()
        {
            throw new System.NotImplementedException();
        }
        
        public void Collect()
        {
            foreach (var command in m_CurrentStat.Commands)
            {
                SavingSystemManager.Instance.OnContributeCommand.Invoke(command);
                SavingSystemManager.Instance.StoreCurrencyAtBuildings(command.ToString(),m_CollectableData.Position);
            }
            
            Debug.Log("Collect this entity");
        }

        public override FactionType GetFaction()
        {
            return m_CollectableData.CreatureType;
        }

        public override int GetExpReward()
        {
            throw new System.NotImplementedException();
        }

        public override void CollectExp(int expAmount)
        {
            throw new System.NotImplementedException();
        }

        public override void TakeDamage(int damage, Entity fromEntity)
        {
            throw new System.NotImplementedException();
        }

        public override int GetCurrentHealth()
        {
            throw new System.NotImplementedException();
        }

        public override void DieIndividualProcess(Entity killedByEntity)
        {
            // TODO add animation or effect here
        }

        public override void AttackSetup(IGetCreatureInfo unitInfo)
        {
            throw new System.NotImplementedException();
        }

        public override int GetAttackDamage()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<Skill_SO> GetSkills()
        {
            throw new System.NotImplementedException();
        }

        public override void SetAnimation(AnimateType animation, bool isTurnOn)
        {
            throw new System.NotImplementedException();
        }

        public override void RefreshEntity()
        {
            // Set entity stats
            m_CurrentStat = m_CollectableStats[m_CollectableData.CurrentLevel];

            // Initiate entity data if it's new
            m_CollectableData.CollectableType = m_CurrentStat.CollectableType;
            // var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_ResourceData.EntityName);
            // m_ResourceData.SkinAddress = inventoryItem.skinAddress[m_ResourceData.CurrentLevel];
            
            // Retrieve entity data
            // m_SkinComp.Init(m_ResourceData.SkinAddress);
            m_CollectComp.Init(OnUnitDie);
            OnUnitDie.AddListener(DieIndividualProcess);
        }
    }
}