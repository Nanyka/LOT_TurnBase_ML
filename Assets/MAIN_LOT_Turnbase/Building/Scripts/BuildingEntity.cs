using System;
using System.Collections.Generic;
using System.Linq;
using GOAP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class BuildingEntity : Entity, IAttackRelated, ISkillRelated, IBuildingUpgrade, IBuildingSale
    {
        // [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private SkillComp m_SkillComp;
        [SerializeField] private FireComp m_FireComp;
        [SerializeField] private AnimateComp m_AnimateComp;
        [SerializeField] private BuildingConstructComp m_Constructor;
        [SerializeField] private UnityEvent OnThisBuildingUpgrade = new();

        private ISkinComp m_SkinComp;
        
        private BuildingData m_BuildingData { get; set; }
        private List<BuildingStats> m_BuildingStats;
        private BuildingStats m_CurrentStats;
        [SerializeField] private int _killAccumulation;

        public void Init(BuildingData buildingData)
        {
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

        public void GainGoldValue()
        {
            throw new NotImplementedException();
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

        public GameObject GetGameObject()
        {
            return gameObject;
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
            m_BuildingData.CurrentStorage +=
                amount <= m_BuildingData.GetStoreSpace() ? amount : m_BuildingData.GetStoreSpace();
            m_HealthComp.UpdateStorage(m_BuildingData.CurrentStorage);
            m_HealthComp.UpdatePriceText(CalculateSellingPrice());
        }

        public void DeductCurrency(int amount)
        {
            m_BuildingData.CurrentStorage -= amount;
            SavingSystemManager.Instance.DeductCurrency(m_BuildingData.StorageCurrency.ToString(), amount);
        }

        public int CalculateSellingPrice()
        {
            return Mathf.RoundToInt((1 + m_CurrentStats.Level) * m_BuildingData.CurrentStorage * 0.1f);
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

        public void TakeDamage(int damage, IAttackRelated fromEntity)
        {
            if (GameFlowManager.Instance.GameMode == GameMode.ECONOMY)
            {
                if (fromEntity.GetFaction() != FactionType.Player)
                    EnemyRopeCurrency(damage);

                if (m_BuildingData.BuildingType != BuildingType.MAINHALL)
                    m_HealthComp.TakeDamage(m_BuildingData, fromEntity);
            }
            else if (GameFlowManager.Instance.GameMode == GameMode.BATTLE)
            {
                // If player's creatures attack enemy building, they also seize loot from this storage
                if (fromEntity.GetFaction() == FactionType.Player && m_BuildingData.FactionType == FactionType.Enemy)
                {
                    var seizedAmount = EnemyRopeCurrency(damage);

                    MainUI.Instance.OnShowCurrencyVfx.Invoke(m_BuildingData.StorageCurrency.ToString(), seizedAmount,
                        m_Transform.position);
                }

                m_HealthComp.TakeDamage(m_BuildingData, fromEntity);
            }
            else
                m_HealthComp.TakeDamage(m_BuildingData, fromEntity);

            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        private int EnemyRopeCurrency(int damage)
        {
            if (m_BuildingData.StorageCurrency == CurrencyType.NONE)
                return 0;

            int seizedAmount = 0;
            if (m_BuildingData.CurrentStorage > m_BuildingData.CurrentHp)
            {
                var damageUpperHealth = Mathf.Clamp(damage * 1f / m_BuildingData.CurrentHp, 0f, 1f);
                seizedAmount = Mathf.RoundToInt(damageUpperHealth * m_BuildingData.CurrentStorage);
            }
            else
                seizedAmount = m_BuildingData.CurrentStorage > damage ? damage : m_BuildingData.CurrentStorage;

            if (GameFlowManager.Instance.GameMode == GameMode.ECONOMY)
                DeductCurrency(seizedAmount);
            else if (GameFlowManager.Instance.GameMode == GameMode.BATTLE)
                MainUI.Instance.OnUpdateCurrencies.Invoke();

            return seizedAmount;
        }

        protected virtual void DieIndividualProcess(IAttackRelated killedByEntity)
        {
            // TODO die visualization
        }

        #endregion

        #region ATTACK

        public virtual void SuccessAttack(GameObject target)
        {
            throw new NotImplementedException();
        }

        public virtual void AttackSetup(IGetEntityInfo unitInfo, IAttackResponse attackResponser)
        {
            if (m_BuildingData.TurnCount > 0)
                return;

            Attack(unitInfo, attackResponser);
        }

        private void Attack(IGetEntityInfo unitInfo, IAttackResponse attackResponser)
        {
            var currenState = unitInfo.GetCurrentState();
            var attackRange = m_SkillComp.AttackPath(currenState.midPos, currenState.direction, currenState.jumpStep);

            m_AttackComp.Attack(attackRange, this, this, currenState.jumpStep);

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

        public EffectComp GetEffectComp()
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
            return m_Constructor;
        }

        #endregion

        #region GENERAL

        protected virtual void RefreshEntity()
        {
            ResetEntity();

            m_Transform.position = m_BuildingData.Position;
            m_HealthComp.Init(m_CurrentStats.MaxHp, OnUnitDie, m_BuildingData);
            m_HealthComp.UpdatePriceText(CalculateSellingPrice());
            m_HealthComp.UpdateStorage(m_BuildingData.CurrentStorage);
            m_SkillComp.Init(m_BuildingData.EntityName);
            OnUnitDie.AddListener(DieIndividualProcess);

            if (GameFlowManager.Instance.GameMode == GameMode.AOE)
                m_Constructor?.Init(m_BuildingData.FactionType);
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
            m_BuildingData.CurrentHp = m_BuildingData.CurrentHp < m_CurrentStats.MaxHp
                ? m_BuildingData.CurrentHp
                : m_CurrentStats.MaxHp;

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

    public interface IBuildingUpgrade
    {
        public BuildingType GetBuildingType();
        public int GetUpgradePrice();
        public CurrencyType GetUpgradeCurrency();
        public int CalculateUpgradePrice();
        public void BuildingUpdate();
        public GameObject GetGameObject();
    }

    public interface IBuildingSale
    {
        public int CalculateSellingPrice();
    }
}