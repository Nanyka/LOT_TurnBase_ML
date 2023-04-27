using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class UnitManager : MonoBehaviour, IStartUpLoadData
    {
        public void StartUpLoadData<T>(T data)
        {
            Debug.Log($"Unit manager loaded {data} data");
        }
    }
}
