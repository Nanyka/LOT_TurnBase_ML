using UnityEngine;

namespace LOT_Turnbase
{
    public class ResourceEntity: Entity
    {
        [Header("Custom components")]
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private EffectComp m_EffectComp;
        
        private ResourceData m_ResourceData;

        public void Init(ResourceData resourceData)
        {
            m_ResourceData = resourceData;
            
            Move(m_ResourceData.Position);
        }

        #region RESOURCE DATA

        private void Move(Vector3 position)
        {
            m_Transform.position = position;
        }

        #endregion
    }
}