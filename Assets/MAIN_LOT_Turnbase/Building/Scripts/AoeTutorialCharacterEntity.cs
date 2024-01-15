using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class AoeTutorialCharacterEntity : AoeTutorialEntity, IAttackRelated
    {
        [HideInInspector] public UnityEvent<IAttackRelated> OnUnitDie = new();
        
        [SerializeField] private CreatureData m_CreatureData;

        private IHealthComp m_HealthComp;
        // private IAttackRegister m_AttackExecutor;

        private void Awake()
        {
            m_HealthComp = GetComponent<IHealthComp>();
            // m_AttackExecutor = GetComponent<IAttackRegister>();
        }
        
        private void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(WaitForSkin);
        }

        private void WaitForSkin()
        {
            Init();
            // gameObject.SetActive(false);
        }

        private void Init()
        {
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_CreatureData.EntityName);
            m_HealthComp.Init(inventoryItem.creatureStats[m_CreatureData.CurrentLevel].HealthPoint, OnUnitDie, m_CreatureData);
            // m_AttackExecutor?.Init();
        }

        public void GainGoldValue()
        {
            throw new NotImplementedException();
        }

        public FactionType GetFaction()
        {
            throw new NotImplementedException();
        }
        
        public int GetAttackDamage()
        {
            return m_CreatureData.CurrentDamage;
        }

        public IEnumerable<Skill_SO> GetSkills()
        {
            throw new NotImplementedException();
        }

        public IEffectComp GetEffectComp()
        {
            throw new NotImplementedException();
        }

        public void AccumulateKills()
        {
            throw new NotImplementedException();
        }
    }
}