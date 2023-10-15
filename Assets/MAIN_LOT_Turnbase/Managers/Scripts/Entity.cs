using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public abstract class Entity: MonoBehaviour
    {
        [NonSerialized] public UnityEvent<Entity> OnUnitDie = new();
        
        [Header("Default components")] 
        [SerializeField] protected Transform m_Transform;
        
        #region ENTITY DATA

        public abstract void Relocate(Vector3 position);
        
        public abstract void UpdateTransform(Vector3 position, Vector3 rotation);

        public abstract EntityData GetData();

        public abstract FactionType GetFaction();

        public abstract void GainGoldValue();

        #endregion
        
        #region HEALTH

        public abstract void TakeDamage(int damage, Entity fromEntity);

        #endregion
        
        #region ATTACK

        public abstract int GetAttackDamage();

        #endregion
        
        #region SKILL
    
        public abstract IEnumerable<Skill_SO> GetSkills();

        #endregion

        #region SKIN

        public abstract SkinComp GetSkin();

        #endregion

        #region EFFECT

        public abstract EffectComp GetEffectComp();

        #endregion
    }
}