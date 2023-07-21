using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectableEntity : Entity
    {
        [SerializeField] private CollectableStats[] m_CollectableStats;
        [SerializeField] private SkinComp m_SkinComp;
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

        public override FactionType GetFaction()
        {
            return m_CollectableData.FactionType;
        }
        
        public bool CheckSelfCollect()
        {
            return m_CurrentStat.IsSelfCollect;
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
            OnUnitDie.RemoveAllListeners();
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

        #region EFFECT
        
        public override EffectComp GetEffectComp()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public override void SetAnimation(AnimateType animateType, bool isTurnOn)
        {
            throw new System.NotImplementedException();
        }

        #region GENERAL
        
        public override void ContributeCommands()
        {
            foreach (var command in m_CurrentStat.Commands)
                SavingSystemManager.Instance.StoreCurrencyAtBuildings(command.ToString(), m_CollectableData.Position);

            if (m_CurrentStat.SpawnedEntityType == EntityType.NONE)
                return;

            switch (m_CurrentStat.SpawnedEntityType)
            {
                case EntityType.BUILDING:
                    SavingSystemManager.Instance.OnPlaceABuilding(m_CurrentStat.EntityName, m_CollectableData.Position,
                        true);
                    break;
                case EntityType.ENEMY:
                    SavingSystemManager.Instance.SpawnMovableEntity(m_CurrentStat.EntityName,m_CollectableData.Position);
                    break;
            }
        }

        public override void RefreshEntity()
        {
            // Set entity stats
            m_CurrentStat = m_CollectableStats[m_CollectableData.CurrentLevel];

            // Initiate entity data if it's new
            m_CollectableData.CollectableType = m_CurrentStat.CollectableType;
            m_CollectableData.SkinAddress = m_CurrentStat.SkinAddress;

            // Retrieve entity data
            m_SkinComp.Init(m_CollectableData.SkinAddress);
            m_CollectComp.Init(OnUnitDie);
            OnUnitDie.AddListener(DieIndividualProcess);
        }

        #endregion
    }
}