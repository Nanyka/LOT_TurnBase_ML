using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class BuildingManager : MonoBehaviour, IStartUpLoadData
    {
        public void StartUpLoadData<T>(T data)
        {
            Debug.Log($"Building manager loaded {data} data");
        }
    }
}
