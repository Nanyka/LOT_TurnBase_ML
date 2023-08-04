using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceEntity : Entity
    {
        [SerializeField] private ResourceStats[] m_ResourceStats;
        [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private AnimateComp m_AnimateComp;

        private ResourceData m_ResourceData;
        private ResourceStats m_CurrentStats;

        public void Init(ResourceData resourceData)
        {
            m_ResourceData = resourceData;
            RefreshEntity();
        }

        // Remove all listener when entity completed die process
        private void OnDisable()
        {
            OnUnitDie.RemoveAllListeners();
        }

        #region RESOURCE DATA

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            m_Transform.position = position;
        }

        public override EntityData GetData()
        {
            return m_ResourceData;
        }

        public void DurationDeduct()
        {
            if (m_CurrentStats.IsLongLasting)
                return;

            m_ResourceData.AccumulatedStep++;
            if (m_ResourceData.AccumulatedStep >= m_CurrentStats.MaxTurnToDestroy)
                OnUnitDie.Invoke(this);
        }

        public override FactionType GetFaction()
        {
            return m_ResourceData.FactionType;
        }

        public virtual int GetExpReward()
        {
            return m_CurrentStats.ExpReward;
        }

        public override void CollectExp(int expAmount)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region HEALTH DATA

        public override void TakeDamage(int damage, Entity fromEntity)
        {
            m_HealthComp.TakeDamage(damage, m_ResourceData, fromEntity);
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public override int GetCurrentHealth()
        {
            throw new System.NotImplementedException();
        }

        public override void DieIndividualProcess(Entity killedByEntity)
        {
            // TODO add animation or effect here
        }

        #endregion

        #region ATTACK

        public override void AttackSetup(IGetCreatureInfo unitInfo, IAttackResponse attackResponser)
        {
            throw new System.NotImplementedException();
        }

        public override int GetAttackDamage()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region SKILL

        public override IEnumerable<Skill_SO> GetSkills()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region EFFECT
        
        public override EffectComp GetEffectComp()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region ANIMATION

        public override void SetAnimation(AnimateType animateType, bool isTurnOn)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region GENERAL

        public override void ContributeCommands()
        {
            foreach (var command in m_CurrentStats.Commands)
                SavingSystemManager.Instance.StoreCurrencyAtBuildings(command.ToString(),m_ResourceData.Position);
        }

        public override void RefreshEntity()
        {
            // Set entity stats
            m_CurrentStats = m_ResourceStats[m_ResourceData.CurrentLevel];

            // Initiate entity data if it's new
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_ResourceData.EntityName);
            m_ResourceData.SkinAddress = inventoryItem.skinAddress[m_ResourceData.CurrentLevel];
            if (m_ResourceData.CurrentHp <= 0)
            {
                m_ResourceData.CurrentHp = m_CurrentStats.MaxHp;
                m_ResourceData.CollectedCurrency = m_CurrentStats.CollectedCurrency;
            }
            
            // Retrieve entity data
            m_SkinComp.Init(m_ResourceData.SkinAddress);
            m_HealthComp.Init(m_CurrentStats.MaxHp, OnUnitDie, m_ResourceData);
            OnUnitDie.AddListener(DieIndividualProcess);
        }

        #endregion
    }
}