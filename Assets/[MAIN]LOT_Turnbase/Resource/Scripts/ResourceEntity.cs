using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceEntity: Entity
    {
        [SerializeField] private ResourceStats m_ResourceStats;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private AnimateComp m_AnimateComp;
        
        private ResourceData m_ResourceData;

        public void Init(ResourceData resourceData)
        {
            m_ResourceData = resourceData;
            RefreshEntity();
        }

        #region RESOURCE DATA
        
        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            m_Transform.position = position;
        }
        
        #endregion

        public override void TakeDamage(int damage)
        {
            throw new System.NotImplementedException();
        }

        public override int GetCurrentHealth()
        {
            throw new System.NotImplementedException();
        }

        public override void AttackSetup(IGetCreatureInfo unitInfo)
        {
            throw new System.NotImplementedException();
        }

        public override int GetAttackDamage()
        {
            throw new System.NotImplementedException();
        }
        
        #region SKILL
        
        public override IEnumerable<Skill_SO> GetSkills()
        {
            throw new System.NotImplementedException();
        }
        
        #endregion

        public override void SetAnimation(AnimateType animation, bool isTurnOn)
        {
            throw new System.NotImplementedException();
        }

        public override void RefreshEntity()
        {
            m_Transform.position = m_ResourceData.Position;
            m_Transform.eulerAngles = m_ResourceData.Rotation;
            m_HealthComp.Init(m_ResourceStats.MaxHp,OnUnitDie,m_ResourceData);
        }
    }
}