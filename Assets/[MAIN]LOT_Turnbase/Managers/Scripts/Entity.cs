using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public abstract class Entity: MonoBehaviour
    {
        [NonSerialized] public UnityEvent<FactionType> OnUnitDie = new();
        
        [Header("Default components")] 
        [SerializeField] protected Transform m_Transform;
        
        #region ENTITY DATA

        public abstract void UpdateTransform(Vector3 position, Vector3 rotation);

        public abstract CommandName GetCommand();

        #endregion
        
        #region HEALTH

        public abstract void TakeDamage(int damage, FactionType fromFaction);

        public abstract int GetCurrentHealth();

        public abstract void DieCollect(FactionType killedByFaction);

        #endregion
        
        #region ATTACK
        
        public abstract void AttackSetup(IGetCreatureInfo unitInfo);

        public abstract int GetAttackDamage();

        #endregion
        
        #region SKILL
    
        public abstract IEnumerable<Skill_SO> GetSkills();

        #endregion

        #region ANIMATE COMPONENT

        public abstract void SetAnimation(AnimateType animation ,bool isTurnOn);

        #endregion

        #region GENERAL

        public abstract void RefreshEntity();

        #endregion
    }
}