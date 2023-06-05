using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingInGame: MonoBehaviour
    {
        [SerializeField] private BuildingEntity m_Entity;
        
        public void Init(BuildingData buildingData)
        {
            m_Entity.Init(buildingData);
            // Debug.Log("Initiate this resource at " + m_ResourceData.Position);
        }
    }
}