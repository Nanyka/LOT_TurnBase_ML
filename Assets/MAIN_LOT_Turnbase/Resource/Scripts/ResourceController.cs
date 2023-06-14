using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceController : MonoBehaviour
    {
        private List<ResourceInGame> m_Resources = new();
        private EnvironmentManager m_Environment;
        
        public void AddResourceToList(ResourceInGame resource)
        {
            m_Resources.Add(resource);
        }

        public void Init()
        {
            m_Environment = GameFlowManager.Instance.GetEnvManager();
            m_Environment.OnChangeFaction.AddListener(DurationDeduct);
        }

        private void DurationDeduct()
        {
            if (m_Environment.GetCurrFaction() != FactionType.Player)
                return;

            foreach (var resource in m_Resources)
                resource.DurationDeduct();
        }

        public void RemoveResource(ResourceInGame resource)
        {
            m_Environment.RemoveObject(resource.gameObject, FactionType.Neutral);
            m_Resources.Remove(resource);
        }
    }
}
