using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceEntity : Entity, IAttackRelated
    {
        [SerializeField] private List<ResourceStats> m_ResourceStats;
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

        public override void Relocate(Vector3 position)
        {
            m_Transform.position = position;
        }

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

        public Vector3 GetPosition()
        {
            throw new NotImplementedException();
        }

        public void GainGoldValue()
        {
            throw new NotImplementedException();
        }

        public override FactionType GetFaction()
        {
            return m_ResourceData.FactionType;
        }

        public int GetAttackDamage()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Skill_SO> GetSkills()
        {
            throw new NotImplementedException();
        }

        public EffectComp GetEffectComp()
        {
            throw new NotImplementedException();
        }

        public void AccumulateKills()
        {
            throw new NotImplementedException();
        }

        // public override void GainGoldValue() { }

        #endregion

        #region HEALTH DATA

        public void TakeDamage(int damage, IAttackRelated fromEntity)
        {
            m_HealthComp.TakeDamage(m_ResourceData, fromEntity);
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public virtual int GetCurrentHealth()
        {
            throw new System.NotImplementedException();
        }

        private void DieIndividualProcess(IAttackRelated killedByEntity)
        {
            // TODO add animation or effect here
        }

        #endregion

        #region SKILL

        // public override IEnumerable<Skill_SO> GetSkills()
        // {
        //     throw new System.NotImplementedException();
        // }

        #endregion

        #region SKIN

        public override SkinComp GetSkin()
        {
            return m_SkinComp;
        }

        #endregion

        #region EFFECT

        // public override EffectComp GetEffectComp()
        // {
        //     throw new System.NotImplementedException();
        // }

        #endregion

        #region ANIMATION

        public virtual void SetAnimation(AnimateType animateType, bool isTurnOn)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region GENERAL

        public virtual void ContributeCommands()
        {
            foreach (var command in m_CurrentStats.Commands)
                SavingSystemManager.Instance.StoreCurrencyAtBuildings(command.ToString(), m_ResourceData.Position);
        }

        public virtual void RefreshEntity()
        {
            // Set entity stats
            var inventory = SavingSystemManager.Instance.GetInventoryItemByName(m_ResourceData.EntityName);
            m_ResourceStats.Clear();
            foreach (var stats in inventory.statsAddress)
                m_ResourceStats.Add(AddressableManager.Instance.GetAddressableSO(stats) as ResourceStats);
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