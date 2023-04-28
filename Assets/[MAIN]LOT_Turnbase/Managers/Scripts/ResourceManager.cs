using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class ResourceManager : MonoBehaviour, IStartUpLoadData
    {
        private List<ResourceInGame> _resources;
        
        private void Start()
        {
            StartUpProcessor.Instance.OnInitiateObjects.AddListener(SpawnResources);
        }

        private void SpawnResources()
        {
            foreach (var resource in _resources)
            {
                resource.Init();
            }
        }

        public void StartUpLoadData<T>(T data)
        {
            _resources = (List<ResourceInGame>)Convert.ChangeType(data, typeof(List<ResourceInGame>));
        }
    }
}
