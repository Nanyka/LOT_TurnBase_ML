using UnityEngine;

namespace LOT_Turnbase
{
    public class ResourceInGame : MonoBehaviour
    {
        [SerializeField] private ResourceType m_ResourceType; // define how resource interact with game
        [SerializeField] private CurrencyType m_CurrencyType; // define what will be as loot for collecting
        [SerializeField] private ResourceEntity m_Entity;

        public void Init(ResourceData resourceData)
        {
            m_Entity.Init(resourceData);
            // Debug.Log("Initiate this resource at " + m_ResourceData.Position);
        }
    }
}