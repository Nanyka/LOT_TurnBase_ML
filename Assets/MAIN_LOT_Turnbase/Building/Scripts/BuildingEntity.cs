using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class BuildingEntity : Entity
    {
        [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private SkillComp m_SkillComp;
        [SerializeField] private FireComp m_FireComp;
        [SerializeField] private AnimateComp m_AnimateComp;
        [SerializeField] private UnityEvent OnThisBuildingUpgrade = new();

        private BuildingData m_BuildingData { get; set; }
        private List<BuildingStats> m_BuildingStats;
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

        public BuildingStats GetStats()
        {
            return m_CurrentStats;
        }

        public override FactionType GetFaction()
        {
            return m_BuildingData.FactionType;
        }

        public BuildingType GetBuildingType()
        {
            return m_BuildingData.BuildingType;
        }

        public void BuildingUpdate()
        {
            if (m_BuildingData.CurrentLevel + 1 >= m_BuildingStats.Count)
                return;

            m_BuildingData.CurrentLevel++;
            m_BuildingData.TurnCount = 0;
            OnThisBuildingUpgrade.Invoke();

            // Reset stats and appearance
            ResetEntity();
        }

        public CurrencyType GetUpgradeCurrency()
        {
            return m_BuildingStats[m_BuildingData.CurrentLevel].UpgradeCurrency;
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
            m_HealthComp.UpdateStorage(m_BuildingData.CurrentStorage);
            m_HealthComp.UpdatePriceText(CalculateSellingPrice());
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
            m_BuildingData.TurnCount = Mathf.Clamp(m_BuildingData.TurnCount - 1, 0, m_BuildingData.TurnCount - 1);
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
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

                MainUI.Instance.OnShowCurrencyVfx.Invoke(m_BuildingData.StorageCurrency.ToString(), seizedAmount,
                    fromEntity.GetData().Position);
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

        public override void AttackSetup(IGetEntityInfo unitInfo, IAttackResponse attackResponser)
        {
            if (m_BuildingData.TurnCount > 0)
                return;

            Attack(unitInfo, attackResponser);
        }

        private void Attack(IGetEntityInfo unitInfo, IAttackResponse attackResponser)
        {
            var currenState = unitInfo.GetCurrentState();
            var attackRange = m_SkillComp.AttackPoints(currenState.midPos, currenState.direction, currenState.jumpStep);

            m_AttackComp.Attack(attackRange, this, currenState.jumpStep);

            ShowAttackRange(attackRange);
            attackResponser.AttackResponse();
        }

        private void ShowAttackRange(IEnumerable<Vector3> attackRange)
        {
            if (attackRange == null)
                return;

            if (attackRange.Any())
            {
                m_FireComp.PlayCurveFX(attackRange);
                m_BuildingData.TurnCount = m_FireComp.GetReloadDuration();
            }
        }

        #endregion

        #region SKILL

        public override IEnumerable<Skill_SO> GetSkills()
        {
            return m_SkillComp.GetSkills();
        }

        #endregion

        #region SKIN

        public override SkinComp GetSkin()
        {
            return m_SkinComp;
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
            return m_BuildingData.CurrentDamage;
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

            m_HealthComp.Init(m_CurrentStats.MaxHp, OnUnitDie, m_BuildingData);
            m_HealthComp.UpdatePriceText(CalculateSellingPrice());
            m_HealthComp.UpdateStorage(m_BuildingData.CurrentStorage);
            m_SkillComp.Init(m_BuildingData.EntityName);
            OnUnitDie.AddListener(DieIndividualProcess);
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

            // Set initiate data if it's new
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_BuildingData.EntityName);
            m_BuildingData.SkinAddress = inventoryItem.skinAddress.Count > m_BuildingData.CurrentLevel
                ? inventoryItem.skinAddress[m_BuildingData.CurrentLevel]
                : inventoryItem.skinAddress[0];
            m_SkinComp.Init(m_BuildingData.SkinAddress);
            if (m_BuildingData.CurrentHp == 0)
                m_BuildingData.CurrentHp = m_CurrentStats.MaxHp;
        }

        #endregion
    }
}