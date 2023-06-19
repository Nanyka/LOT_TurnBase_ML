using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class BuildingEntity: Entity
    {
        [SerializeField] private BuildingStats m_BuildingStats;
        [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private StorageComp m_StorageComp;
        [SerializeField] private LevelComp m_LevelComp;
        [SerializeField] private AttackPath m_AttackPath;
        [SerializeField] private AnimateComp m_AnimateComp;
        
        private BuildingData m_BuildingData { get; set; }
        
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
        
        public override CommandName GetCommand()
        {
            throw new NotImplementedException();
        }

        public override FactionType GetFaction()
        {
            return m_BuildingData.CreatureType;
        }

        public override int GetExpReward()
        {
            throw new NotImplementedException();
        }

        public override void CollectExp(int expAmount)
        {
            throw new NotImplementedException();
        }

        public int GetStorageSpace(CurrencyType currencyType, ref Queue<BuildingEntity> selectedBuildings)
        {
            if (currencyType != m_BuildingStats.StoreCurrency)
                return 0;

            selectedBuildings.Enqueue(this);
            return m_BuildingData.StorageCapacity - m_BuildingData.CurrentStorage;
        }
        
        public int GetStorageSpace(CurrencyType currencyType)
        {
            if (currencyType != m_BuildingStats.StoreCurrency)
                return 0;

            return m_BuildingData.StorageCapacity - m_BuildingData.CurrentStorage;
        }

        public void StoreCurrency(int amount)
        {
            m_BuildingData.CurrentStorage += amount;
            m_BuildingData.CurrentExp += amount;
        }

        public int CalculateSellingPrice()
        {
            return m_BuildingStats.Level * m_BuildingData.TurnCount;
        }
        
        public void DurationDeduct()
        {
            m_BuildingData.TurnCount++;
        }
        
        #endregion

        #region HEALTH

        public override void TakeDamage(int damage, Entity fromEntity)
        {
            m_HealthComp.TakeDamage(damage, m_BuildingData, fromEntity);
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public override int GetCurrentHealth()
        {
            throw new NotImplementedException();
        }

        public override void DieCollect(Entity killedByEntity)
        {
            throw new NotImplementedException();
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

        public override void RefreshEntity()
        {
            // Set default information to buildingData
            m_BuildingData.StorageCapacity = m_BuildingStats.StorageCapacity;
            m_BuildingData.StorageCurrency = m_BuildingData.StorageCurrency;
            
            // Set initiate data if it's new
            if (m_BuildingData.CurrentHp == 0)
            {
                m_BuildingData.CurrentHp = m_BuildingStats.MaxHp;
                m_BuildingData.StorageCurrency = m_BuildingStats.StoreCurrency;
                m_BuildingData.StorageCapacity = m_BuildingStats.StorageCapacity;
            }
            
            // Load data to entity
            m_Transform.position = m_BuildingData.Position;
            m_Transform.eulerAngles = m_BuildingData.Rotation;
            m_SkinComp.Initiate(m_BuildingData.SkinAddress);
            m_HealthComp.Init(m_BuildingStats.MaxHp,OnUnitDie,m_BuildingData);
            OnUnitDie.AddListener(DieCollect);
        }
    }
}