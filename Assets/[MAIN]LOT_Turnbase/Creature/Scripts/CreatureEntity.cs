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
    }
}
