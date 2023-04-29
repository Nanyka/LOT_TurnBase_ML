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
            StartUpProcessor.Instance.OnInitiateObjects.AddListener(Init);
        }
        
        // Prepare data for game session
        public void StartUpLoadData<T>(T data)
        {
            _resources = (List<ResourceInGame>)Convert.ChangeType(data, typeof(List<ResourceInGame>));
        }

        private void Init()
        {
            foreach (var resource in _resources)
            {
                resource.Init();
            }
        }
    }
}
