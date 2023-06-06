using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceInGame : MonoBehaviour, IRemoveEntity
    {
        [SerializeField] private ResourceType m_ResourceType; // define how resource interact with game
        [SerializeField] private ResourceEntity m_Entity;

        private ResourceController _resourceController;

        public void Init(ResourceData resourceData, ResourceController resourceController)
        {
            m_Entity.Init(resourceData);
            m_Entity.OnUnitDie.AddListener(DestroyResource);

            _resourceController = resourceController;
            _resourceController.AddResourceToList(this);
        }

        public void DurationDeduct()
        {
            m_Entity.DurationDeduct();
        }

        private void DestroyResource()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);
            _resourceController.RemoveResource(this);
            StartCoroutine(DestroyVisual());
        }
        
        private IEnumerator DestroyVisual()
        {
            // Collect currencies
            // VFX
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }

        public void Remove(EnvironmentData environmentData)
        {
            environmentData._testResourceData.Remove(m_Entity.GetResourceData());
        }
    }
}