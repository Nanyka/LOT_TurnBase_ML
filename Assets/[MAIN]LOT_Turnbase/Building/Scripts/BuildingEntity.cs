using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
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

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
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
            m_Transform.position = m_BuildingData.Position;
            m_Transform.eulerAngles = m_BuildingData.Rotation;
            m_HealthComp.Init(m_BuildingStats.MaxHp,OnUnitDie,m_BuildingData);
        }
    }
}