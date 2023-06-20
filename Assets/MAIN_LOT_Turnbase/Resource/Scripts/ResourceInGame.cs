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

        private void DestroyResource(Entity killedByEntity)
        {
            // just contribute resource when it is killed by player faction
            if (killedByEntity.GetFaction() == FactionType.Player)
            {
                SavingSystemManager.Instance.OnContributeCommand.Invoke(m_Entity.GetCommand());
                SavingSystemManager.Instance.StoreCurrencyAtBuildings(m_Entity.GetCommand().ToString(),m_Entity.GetData().Position);
            }
            
            // Add exp for entity who killed this resource
            if (killedByEntity != m_Entity)
                killedByEntity.CollectExp(m_Entity.GetExpReward());
            
            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);
            StartCoroutine(DestroyVisual());
        }

        private IEnumerator DestroyVisual()
        {
            // Collect currencies
            // VFX
            yield return new WaitForSeconds(1f);
            _resourceController.RemoveResource(this);
            gameObject.SetActive(false);
        }

        public void Remove(EnvironmentData environmentData)
        {
            environmentData.ResourceData.Remove((ResourceData)m_Entity.GetData());
        }
    }
}