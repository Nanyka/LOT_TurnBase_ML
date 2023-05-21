using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class CreatureEntity : Entity
    {
        [Header("Custom components")]
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
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

        private void Move(Vector3 position)
        {
            m_Transform.position = position;
        }

        #endregion
        
        #region HEALTH

        // public void TakeDamage(int damage)
        // {
        //     m_HealthComp.TakeDamage(damage, ref data);
        // }
        //
        // public int GetCurrentHealth()
        // {
        //     return m_HealthComp.GetCurrentHealth(ref data);
        // }

        #endregion
        
        #region ATTACK
        
        public void Attack(IGetCreatureInfo unitInfo)
        {
            // m_Info = unitInfo;
            // _currentState = unitInfo.GetCurrentState();
            //
            // m_Animator.SetTrigger(m_UnitSkill.GetAttackAnimation(_currentState.jumpStep-1));
        }

        #endregion

        #region ANIMATE COMPONENT

        public void SetAnimation(AnimateType animation ,bool isTurnOn)
        {
            m_AnimateComp.SetAnimation(animation,isTurnOn);
        }

        #endregion

        #region GENERAL

        public void ResetEntity()
        {
            Debug.Log("Reset Health");
            // m_HealthComp.Reset(ref data);
        }

        #endregion
    }
}
