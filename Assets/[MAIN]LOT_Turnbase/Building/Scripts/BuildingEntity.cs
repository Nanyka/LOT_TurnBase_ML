using System;
using UnityEngine;
using UnityEngine.Events;

namespace LOT_Turnbase
{
    public class BuildingEntity: Entity
    {
        [Header("Custom components")]
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private StorageComp m_StorageComp;
        [SerializeField] private LevelComp m_LevelComp;
        [SerializeField] private AttackPath m_AttackPath;
        
        private BuildingData m_BuildingData;
        
        public void Init(BuildingData buildingData)
        {
            m_BuildingData = buildingData;
        }
    }
}