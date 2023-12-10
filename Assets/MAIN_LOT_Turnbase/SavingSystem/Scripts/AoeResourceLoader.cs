using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeResourceLoader : MonoBehaviour, IResourceLoader
    {
        [SerializeField] private ObjectPool _resoucePool;
        
        private ResourceController _resourceController;
        private List<ResourceData> _resourceData = new();
        
        private void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _resourceController = GetComponent<ResourceController>();
        }
        
        public void Init()
        {
            foreach (var resource in _resourceData)
            {
                SpawnResource(resource);
            }

            // _resourceController.Init();
        }

        public void StartUpLoadData(List<ResourceData> data)
        {
            _resourceData = data;
        }

        public GameObject PlaceNewObject(ResourceData data)
        {
            return SpawnResource(data);
        }
        
        private GameObject SpawnResource(ResourceData resourceData)
        {
            var resourceObj = _resoucePool.GetObject(resourceData.EntityName);
            resourceData.FactionType = FactionType.Neutral; // assign Faction
            resourceData.EntityType = EntityType.RESOURCE;
            GameFlowManager.Instance.OnDomainRegister.Invoke(resourceObj, resourceData.FactionType);

            if (resourceObj.TryGetComponent(out ResourceInGame resourceInGame))
            {
                resourceInGame.gameObject.SetActive(true);
                resourceInGame.Init(resourceData, _resourceController);
            }

            return resourceObj;
        }

        public IEnumerable<GameObject> GetResources()
        {
            return _resoucePool.GetActiveItemList();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}