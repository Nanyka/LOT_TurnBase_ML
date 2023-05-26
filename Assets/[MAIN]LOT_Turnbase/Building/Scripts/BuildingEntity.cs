using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LOT_Turnbase
{
    public class BuildingEntity: Entity
    {
        [SerializeField] private BuildingStats m_BuildingStats;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private StorageComp m_StorageComp;
        [SerializeField] private LevelComp m_LevelComp;
        [SerializeField] private AttackPath m_AttackPath;
        [SerializeField] private AnimateComp m_AnimateComp;
        
        private BuildingData m_BuildingData;
        
        public void Init(BuildingData buildingData)
        {
            m_BuildingData = buildingData;
            RefreshEntity();
        }

        protected override void Move(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public override void TakeDamage(int damage)
        {
            throw new NotImplementedException();
        }

        public override int GetCurrentHealth()
        {
            throw new NotImplementedException();
        }

        public override void AttackSetup(IGetCreatureInfo unitInfo)
        {
            throw new NotImplementedException();
        }
        
        #region SKILL
        
        public override IEnumerable<Skill_SO> GetSkills()
        {
            throw new System.NotImplementedException();
        }
        
        #endregion

        public override int GetAttackDamage()
        {
            throw new NotImplementedException();
        }

        public override void SetAnimation(AnimateType animation, bool isTurnOn)
        {
            throw new NotImplementedException();
        }

        public override void RefreshEntity()
        {
            m_HealthComp.Init(m_BuildingStats.MaxHp,OnUnitDie,m_BuildingData);
        }
    }
}