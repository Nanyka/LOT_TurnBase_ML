using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class BuildingEntity : Entity
    {
        [SerializeField] private BuildingStats[] m_BuildingStats;
        [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private StorageComp m_StorageComp;
        [SerializeField] private LevelComp m_LevelComp;
        [SerializeField] private AttackPath m_AttackPath;
        [SerializeField] private AnimateComp m_AnimateComp;

        private BuildingData m_BuildingData { get; set; }
        private BuildingStats m_CurrentStats;

        public void Init(BuildingData buildingData)
        {
            m_BuildingData = buildingData;
            RefreshEntity();
        }

        #region BUILDING DATA

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            throw new NotImplementedException();
        }

        public override EntityData GetData()
        {
            return m_BuildingData;
        }

        public override FactionType GetFaction()
        {
            return m_BuildingData.FactionType;
        }

        public override int GetExpReward()
        {
            return m_CurrentStats.ExpReward;
        }

        public override void CollectExp(int expAmount)
        {
            m_BuildingData.CurrentExp += expAmount;
            if (m_BuildingData.CurrentExp >= m_CurrentStats.ExpToUpdate &&
                m_BuildingData.CurrentLevel + 1 < m_BuildingStats.Length)
            {
                // Level up
                m_BuildingData.CurrentLevel++;
                m_BuildingData.CurrentExp = 0;
                m_BuildingData.TurnCount = 0;

                // Reset stats and appearance
                ResetEntity();
                SavingSystemManager.Instance.OnCheckExpandMap.Invoke();
            }
        }

        public int GetStorageSpace(CurrencyType currencyType, ref List<BuildingEntity> selectedBuildings)
        {
            if (currencyType == m_CurrentStats.StoreCurrency || m_CurrentStats.StoreCurrency == CurrencyType.MULTI)
            {
                selectedBuildings.Add(this);
                return m_BuildingData.StorageCapacity - m_BuildingData.CurrentStorage;
            }

            return 0;
        }

        public int GetStorageSpace(CurrencyType currencyType)
        {
            if (currencyType == m_CurrentStats.StoreCurrency || m_CurrentStats.StoreCurrency == CurrencyType.MULTI)
                return m_BuildingData.StorageCapacity - m_BuildingData.CurrentStorage;

            return 0;
        }

        public void StoreCurrency(int amount)
        {
            m_BuildingData.CurrentStorage += amount;
            CollectExp(amount);
        }

        public int CalculateSellingPrice()
        {
            return m_CurrentStats.Level * m_BuildingData.TurnCount;
        }

        public int CalculateUpgradePrice()
        {
            return m_CurrentStats.ExpToUpdate - m_BuildingData.CurrentExp;
        }

        public void DurationDeduct()
        {
            m_BuildingData.TurnCount++;
        }

        #endregion

        #region HEALTH

        public override void TakeDamage(int damage, Entity fromEntity)
        {
            // If player's creatures attack enemy building, they also seize loot from this storage
            if (fromEntity.GetFaction() == FactionType.Player && m_BuildingData.FactionType == FactionType.Enemy)
            {
                var damageUpperHealth = Mathf.Clamp(damage * 1f / m_BuildingData.CurrentHp*1f,0f,1f);
                var seizedAmount = Mathf.RoundToInt(damageUpperHealth * m_BuildingData.CurrentStorage);
                SavingSystemManager.Instance.IncrementLocalCurrency(m_BuildingData.StorageCurrency.ToString(), seizedAmount);
                m_BuildingData.CurrentStorage -= seizedAmount;
            }
            
            m_HealthComp.TakeDamage(damage, m_BuildingData, fromEntity);
            
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public override int GetCurrentHealth()
        {
            throw new NotImplementedException();
        }

        public override void DieIndividualProcess(Entity killedByEntity)
        {
            OnUnitDie.RemoveAllListeners();
            // TODO die visualization
        }

        #endregion

        #region ATTACK

        public override void AttackSetup(IGetCreatureInfo unitInfo)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SKILL

        public override IEnumerable<Skill_SO> GetSkills()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region ANIMATION

        public override int GetAttackDamage()
        {
            throw new NotImplementedException();
        }

        public override void SetAnimation(AnimateType animation, bool isTurnOn)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GENERAL

        public override void ContributeCommands()
        {
            // TODO contribute its resource based on storage amount
            throw new NotImplementedException();
        }

        public override void RefreshEntity()
        {
            ResetEntity();

            // Load data to entity
            m_SkinComp.Init(m_BuildingData.SkinAddress);
            m_HealthComp.Init(m_CurrentStats.MaxHp, OnUnitDie, m_BuildingData);
            OnUnitDie.AddListener(DieIndividualProcess);

            // Check expand map
            SavingSystemManager.Instance.OnCheckExpandMap.Invoke();
        }

        private void ResetEntity()
        {
            // Set entity stats
            m_CurrentStats = m_BuildingStats[m_BuildingData.CurrentLevel];

            // Set default information to buildingData
            m_BuildingData.BuildingType = m_CurrentStats.BuildingType;
            m_BuildingData.StorageCurrency = m_CurrentStats.StoreCurrency;
            m_BuildingData.StorageCapacity = m_CurrentStats.StorageCapacity;
            m_BuildingData.CurrentDamage = m_CurrentStats.AttackDamage;
            m_BuildingData.CurrentShield = m_CurrentStats.Shield;

            // Set initiate data if it's new
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_BuildingData.EntityName);
            m_BuildingData.SkinAddress = inventoryItem.skinAddress.Count > m_BuildingData.CurrentLevel
                ? inventoryItem.skinAddress[m_BuildingData.CurrentLevel]
                : inventoryItem.skinAddress[0];
            if (m_BuildingData.CurrentHp == 0)
            {
                m_BuildingData.CurrentHp = m_CurrentStats.MaxHp;
            }
        }

        #endregion
    }
}