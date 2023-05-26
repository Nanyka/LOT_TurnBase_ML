using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class ResourceManager : MonoBehaviour, IStartUpLoadData
    {
        [SerializeField] private ObjectPool _resoucePool;
        
        private List<ResourceData> _resourceDatas;
        
        private void Start()
        {
            StartUpProcessor.Instance.OnInitiateObjects.AddListener(Init);
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
                StartUpProcessor.Instance.OnDomainRegister.Invoke(resourceObj, FactionType.Neutral);

                if (resourceObj.TryGetComponent(out ResourceInGame resourceInGame))
                {
                    resourceInGame.gameObject.SetActive(true);
                    resourceInGame.Init(resource);
                }
            }
        }
    }
}
