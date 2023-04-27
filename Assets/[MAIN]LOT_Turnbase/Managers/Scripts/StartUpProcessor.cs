using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LOT_Turnbase
{
    public class StartUpProcessor : Singleton<StartUpProcessor>
    {
        [NonSerialized] public UnityEvent OnLoadData = new(); // sent to DataManager
        [NonSerialized] public UnityEvent OnInitiateObjects = new(); // send to Managers; invoke from TileManager

        private void Start()
        {
            OnLoadData.Invoke();
        }
    }
}
