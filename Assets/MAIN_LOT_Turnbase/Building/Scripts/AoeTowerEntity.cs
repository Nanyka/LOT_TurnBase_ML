using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTowerEntity: Entity, IAttackRelated, ISkillCaster, IBuildingDealer, IGetEntityData<BuildingStats>
    {
        // [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private SkillComp m_SkillComp;

        private IHealthComp m_HealthComp;
        private IBuildingConstruct m_Constructor;
        private ISkinComp m_SkinComp;
        private BuildingData m_BuildingData;
        private List<BuildingStats> m_BuildingStats;
        private BuildingStats m_CurrentStats;
        private int _killAccumulation;

        public void Init(BuildingData buildingData, IBuildingController buildingController)
        {
            m_HealthComp = GetComponent<IHealthComp>();
            m_Constructor = GetComponent<IBuildingConstruct>();
            m_SkinComp = GetComponent<ISkinComp>();
            m_BuildingData = buildingData;
            RefreshEntity();
        }

        // Remove all listener when entity completed die process
        private void OnDisable()
        {
            OnUnitDie.RemoveAllListeners();
        }

        #region BUILDING DATA

        public override void Relocate(Vector3 position)
        {
            m_Transform.position = position;
        }

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            m_Transform.position = position;
            m_Transform.eulerAngles = rotation;

            m_BuildingData.Position = position;
            m_BuildingData.Rotation = rotation;
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public override EntityData GetData()
        {
            return m_BuildingData;
        }

        public BuildingStats GetStats()
        {
            return m_CurrentStats;
        }

        public bool CheckAvailable()
        {
            return m_HealthComp.CheckAlive();
        }

        public void GainGoldValue()
        {
            throw new NotImplementedException();
        }

        public override FactionType GetFaction()
        {
            return m_BuildingData.FactionType;
        }

        public int CalculateSellingPrice()
        {
            return Mathf.RoundToInt((1 + m_CurrentStats.Level) * m_BuildingData.CurrentStorage * 0.1f);
        }

        #endregion

        #region HEALTH

        protected virtual void DieIndividualProcess(IAttackRelated killedByEntity)
        {
            // TODO die visualization
        }

        #endregion

        #region ATTACK

        public int GetAttackDamage()
        {
            return m_BuildingData.CurrentDamage;
        }

        #endregion

        #region SKILL
        
        public IEnumerable<Skill_SO> GetSkills()
        {
            return m_SkillComp.GetSkills();
        }

        #endregion

        #region SKIN

        public override ISkinComp GetSkin()
        {
            return m_SkinComp;
        }

        #endregion

        #region EFFECT

        public IEffectComp GetEffectComp()
        {
            throw new NotImplementedException();
        }

        public void AccumulateKills()
        {
            // TODO: Reset kill accumulation after executing critical skill
            _killAccumulation++;
        }

        #endregion

        #region GOAP relevant

        public IChangeWorldState GetWorldStateChanger()
        {
            return m_Constructor.GetWorldStateChanger();
        }

        #endregion

        #region GENERAL

        protected virtual void RefreshEntity()
        {
            ResetEntity();

            m_Transform.position = m_BuildingData.Position;
            m_HealthComp.Init(m_CurrentStats.MaxHp, OnUnitDie, m_BuildingData);
            m_SkillComp.Init(m_BuildingData.EntityName);
            OnUnitDie.AddListener(DieIndividualProcess);
            m_Constructor.Init(m_BuildingData.FactionType);
        }

        private void ResetEntity()
        {
            // Set entity stats
            var inventory = SavingSystemManager.Instance.GetInventoryItemByName(m_BuildingData.EntityName);
            m_BuildingStats = inventory.buildingStats;
            m_CurrentStats = m_BuildingStats[m_BuildingData.CurrentLevel];

            // Set default information to buildingData
            m_BuildingData.BuildingType = m_CurrentStats.BuildingType;
            m_BuildingData.StorageCurrency = m_CurrentStats.StoreCurrency;
            m_BuildingData.StorageCapacity = m_CurrentStats.StorageCapacity;
            m_BuildingData.CurrentDamage = m_CurrentStats.AttackDamage;
            m_BuildingData.CurrentShield = m_CurrentStats.Shield;
            m_BuildingData.CurrentHp = m_CurrentStats.MaxHp;

            // Debug.Log($"Building type of {m_BuildingData.EntityName}: {m_BuildingData.BuildingType}");

            // Set initiate data if it's new
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_BuildingData.EntityName);
            m_BuildingData.SkinAddress = inventoryItem.skinAddress.Count > m_BuildingData.CurrentLevel
                ? inventoryItem.skinAddress[m_BuildingData.CurrentLevel]
                : inventoryItem.skinAddress[0];
            m_SkinComp.Init(m_BuildingData.SkinAddress);
        }

        #endregion
    }
}