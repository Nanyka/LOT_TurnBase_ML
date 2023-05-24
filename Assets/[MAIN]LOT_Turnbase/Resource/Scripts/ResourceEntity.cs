using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class ResourceEntity: Entity
    {
        [Header("Custom components")]
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private AnimateComp m_AnimateComp;
        
        private ResourceData m_ResourceData;

        public void Init(ResourceData resourceData)
        {
            m_ResourceData = resourceData;
            
            Move(m_ResourceData.Position);
        }

        #region RESOURCE DATA

        protected override void Move(Vector3 position)
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

        public override void Attack(IGetCreatureInfo unitInfo)
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

        public override void ResetEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}