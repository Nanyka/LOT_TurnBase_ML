using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceLoader : MonoBehaviour, ILoadData
    {
        [SerializeField] private ObjectPool _resoucePool;

        private ResourceController _resourceController;
        private List<ResourceData> _resourceDatas;
        
        private void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _resourceController = GetComponent<ResourceController>();
        }
        
        // Prepare data for game session
        public void StartUpLoadData<T>(T data)
        {
            _resourceDatas = (List<ResourceData>)Convert.ChangeType(data, typeof(List<ResourceData>));
        }

        private void Init()
        {
            foreach (var resource in _resourceDatas)
            {
                SpawnResource(resource);
            }

            _resourceController.Init();
        }

        public void PlaceNewObject<T>(T data)
        {
            var resourceData = (ResourceData)Convert.ChangeType(data, typeof(ResourceData));
            SpawnResource(resourceData);
        }

        private void SpawnResource(ResourceData resource)
        {
            var resourceObj = _resoucePool.GetObject();
            GameFlowManager.Instance.OnDomainRegister.Invoke(resourceObj, resource.CreatureType);

            if (resourceObj.TryGetComponent(out ResourceInGame resourceInGame))
            {
                resourceInGame.gameObject.SetActive(true);
                resourceInGame.Init(resource, _resourceController);
            }
        }


        public void Reset()
        {
            _resoucePool.ResetPool();
            _resourceDatas = new();
        }
    }
}
