using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceLoader : MonoBehaviour, IResourceLoader
    {
        [SerializeField] private ObjectPool _resoucePool;

        private ResourceController _resourceController;
        private List<ResourceData> _resourceDatas = new();
        
        private void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _resourceController = GetComponent<ResourceController>();
        }
        
        // Prepare data for game session
        public void StartUpLoadData(List<ResourceData> data)
        {
            _resourceDatas = (List<ResourceData>)Convert.ChangeType(data, typeof(List<ResourceData>));
        }

        public void Init()
        {
            foreach (var resource in _resourceDatas)
            {
                SpawnResource(resource);
            }

            _resourceController.Init();
        }

        public GameObject PlaceNewObject(ResourceData data)
        {
            // var resourceData = (ResourceData)Convert.ChangeType(data, typeof(ResourceData));
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
        
        public void Reset()
        {
            _resoucePool.ResetPool();
            _resourceDatas = new();
        }

        public IEnumerable<GameObject> GetResources()
        {
            return _resoucePool.GetActiveItemList();
            // return _resourceController.GetResources();
        }
    }

    public interface IResourceLoader
    {
        public void Init();
        public void StartUpLoadData(List<ResourceData> data);
        public GameObject PlaceNewObject(ResourceData data);
        public IEnumerable<GameObject> GetResources();
        public void Reset();
    }
}
