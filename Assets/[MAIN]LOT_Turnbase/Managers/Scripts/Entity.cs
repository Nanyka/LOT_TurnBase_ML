using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LOT_Turnbase
{
    public abstract class Entity: MonoBehaviour
    {
        [NonSerialized] public UnityEvent OnUnitDie = new();
        
        [Header("Default components")] 
        [SerializeField] protected Transform m_Transform;
        
        #region CREATURE DATA

        protected abstract void Move(Vector3 position);

        #endregion
        
        #region HEALTH

        public abstract void TakeDamage(int damage);

        public abstract int GetCurrentHealth();

        #endregion
        
        #region ATTACK
        
        public abstract void Attack(IGetCreatureInfo unitInfo);

        public abstract int GetAttackDamage();

        #endregion
        
        #region SKILL
    
        public abstract IEnumerable<Skill_SO> GetSkills();

        #endregion

        #region ANIMATE COMPONENT

        public abstract void SetAnimation(AnimateType animation ,bool isTurnOn);

        #endregion

        #region GENERAL

        public abstract void ResetEntity();

        #endregion
    }
}