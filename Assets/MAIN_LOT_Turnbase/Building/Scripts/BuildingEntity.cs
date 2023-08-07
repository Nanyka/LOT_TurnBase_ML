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

        public override FactionType GetFaction()
        {
            return m_BuildingData.FactionType;
        }

        public virtual int GetExpReward()
        {
            return m_CurrentStats.ExpReward;
        }

        public override void CollectExp(int expAmount)
        {
            m_BuildingData.CurrentExp += expAmount;
        }

        public void BuildingUpdate()
        {
            if (m_BuildingData.CurrentLevel + 1 >= m_BuildingStats.Length)
                return;
            
            m_BuildingData.CurrentLevel++;
            m_BuildingData.TurnCount = 0;

            // Reset stats and appearance
            ResetEntity();
        }

        public int GetUpgradePrice()
        {
            return m_BuildingStats[m_BuildingData.CurrentLevel].PriceToUpdate;
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

        public int GetCurrentStorage(CurrencyType currencyType, ref List<BuildingEntity> selectedBuildings)
        {
            if (currencyType == m_CurrentStats.StoreCurrency || m_CurrentStats.StoreCurrency == CurrencyType.MULTI)
            {
                selectedBuildings.Add(this);
                return m_BuildingData.CurrentStorage;
            }

            return 0;
        }

        public int GetCurrentStorage(CurrencyType currencyType)
        {
            if (currencyType == m_CurrentStats.StoreCurrency || m_CurrentStats.StoreCurrency == CurrencyType.MULTI)
                return m_BuildingData.CurrentStorage;

            return 0;
        }

        public void StoreCurrency(int amount)
        {
            m_BuildingData.CurrentStorage += amount;
            CollectExp(amount);
        }

        public void DeductCurrency(int amount)
        {
            m_BuildingData.CurrentStorage -= amount;
        }

        public int CalculateSellingPrice()
        {
            return m_CurrentStats.Level * m_BuildingData.CurrentExp;
        }

        public int CalculateUpgradePrice()
        {
            return m_CurrentStats.PriceToUpdate;
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
                var damageUpperHealth = Mathf.Clamp(damage * 1f / m_BuildingData.CurrentHp * 1f, 0f, 1f);
                var seizedAmount = Mathf.RoundToInt(damageUpperHealth * m_BuildingData.CurrentStorage);
                m_BuildingData.CurrentStorage -= seizedAmount;

                // Storage currency require player's envData that is retrieved from SavingSystemManager.Instance.GetEnvDataForSave() in BattleMode
                SavingSystemManager.Instance.StoreCurrencyByEnvData(m_BuildingData.StorageCurrency.ToString(),
                    seizedAmount, SavingSystemManager.Instance.GetEnvDataForSave());
                // SavingSystemManager.Instance.IncrementLocalCurrency(m_BuildingData.StorageCurrency.ToString(), seizedAmount);
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
            // TODO die visualization
        }

        #endregion

        #region ATTACK

        public override void AttackSetup(IGetCreatureInfo unitInfo, IAttackResponse attackResponser)
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

        #region EFFECT

        public override EffectComp GetEffectComp()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ANIMATION

        public override int GetAttackDamage()
        {
            throw new NotImplementedException();
        }

        public override void SetAnimation(AnimateType animateType, bool isTurnOn)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GENERAL

        public override void ContributeCommands()
        {
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
            // SavingSystemManager.Instance.OnCheckExpandMap.Invoke();
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