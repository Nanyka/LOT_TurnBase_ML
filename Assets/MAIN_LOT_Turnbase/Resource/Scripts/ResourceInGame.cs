using System;
using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceInGame : MonoBehaviour, IRemoveEntity
    {
        [SerializeField] private ResourceEntity m_Entity;

        private ResourceController _resourceController;

        private void OnEnable()
        {
            m_Entity.OnUnitDie.AddListener(DestroyResource);
        }

        private void OnDisable()
        {
            m_Entity.OnUnitDie.RemoveListener(DestroyResource);
        }

        public void Init(ResourceData resourceData, ResourceController resourceController)
        {
            m_Entity.Init(resourceData);
            transform.position = resourceData.Position;

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
                m_Entity.ContributeCommands();
            
            // Add exp for entity who killed this resource
            if (killedByEntity != m_Entity)
                killedByEntity.CollectExp(m_Entity.GetExpReward());
            
            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);
            StartCoroutine(DestroyVisual());
        }

        private IEnumerator DestroyVisual()
        {
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