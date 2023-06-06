using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceLoader : MonoBehaviour, IStartUpLoadData
    {
        [SerializeField] private ObjectPool _resoucePool;

        private ResourceController _resourceController;
        [SerializeField] private List<ResourceData> _resourceDatas;
        
        private void Start()
        {
            StartUpProcessor.Instance.OnInitiateObjects.AddListener(Init);
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
                var resourceObj = _resoucePool.GetObject();
                StartUpProcessor.Instance.OnDomainRegister.Invoke(resourceObj, resource.CreatureType);

                if (resourceObj.TryGetComponent(out ResourceInGame resourceInGame))
                {
                    resourceInGame.gameObject.SetActive(true);
                    resourceInGame.Init(resource, _resourceController);
                }
            }

            _resourceController.Init();
        }

        public void Reset()
        {
            _resoucePool.ResetPool();
            _resourceDatas = new();
        }
    }
}
