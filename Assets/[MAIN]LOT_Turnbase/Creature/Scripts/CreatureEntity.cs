using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LOT_Turnbase
{
    public class CreatureEntity : Entity
    {
        [Header("Custom components")]
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
        [SerializeField] private SkillComp m_SkillComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private LevelComp m_LevelComp;
        [SerializeField] private AttackPath m_AttackPath;
        [SerializeField] private AnimateComp m_AnimateComp;
        
        private CreatureData m_CreatureData;
        private IGetUnitInfo m_Info;
        
        public void Init(CreatureData creatureData)
        {
            m_CreatureData = creatureData;
            Move(m_CreatureData.Position);
        }
        
        #region CREATURE DATA

        protected override void Move(Vector3 position)
        {
            m_Transform.position = position;
        }

        #endregion
        
        #region HEALTH

        public override void TakeDamage(int damage)
        {
            m_HealthComp.TakeDamage(damage, m_CreatureData);
        }
        
        public override int GetCurrentHealth()
        {
            return m_HealthComp.GetCurrentHealth(m_CreatureData);
        }

        #endregion
        
        #region ATTACK
        
        public override void Attack(IGetCreatureInfo unitInfo)
        {
            Debug.Log($"{gameObject} attack");
            // m_Info = unitInfo;
            // _currentState = unitInfo.GetCurrentState();
            //
            // m_Animator.SetTrigger(m_UnitSkill.GetAttackAnimation(_currentState.jumpStep-1));
        }

        public void ShowAttackRange(IEnumerable<Vector3> attackRange)
        {
            Debug.Log($"{gameObject} show attack range");
        }

        public override int GetAttackDamage()
        {
            throw new System.NotImplementedException();
        }

        #endregion
        
        #region SKILL
        
        public override IEnumerable<Skill_SO> GetSkills()
        {
            return m_SkillComp.GetSkills();
        }
        
        #endregion

        #region ANIMATE COMPONENT

        public override void SetAnimation(AnimateType animation ,bool isTurnOn)
        {
            m_AnimateComp.SetAnimation(animation,isTurnOn);
        }

        #endregion

        #region GENERAL

        public override void ResetEntity()
        {
            Debug.Log("Reset Health");
            // m_HealthComp.Reset(ref data);
        }

        #endregion
    }
}
