using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class BuildingManager : MonoBehaviour, IStartUpLoadData
    {
        private void Start()
        {
            StartUpProcessor.Instance.OnInitiateObjects.AddListener(Init);
        }

        public void StartUpLoadData<T>(T data)
        {
            Debug.Log($"Building manager loaded {data} data");
        }
        
        private void Init()
        {
            Debug.Log("Spawn buildings");
        }
    }
}
