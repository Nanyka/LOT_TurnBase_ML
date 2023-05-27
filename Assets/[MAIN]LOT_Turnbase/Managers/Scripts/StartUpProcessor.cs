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
        [NonSerialized] public UnityEvent<Vector3> OnUpdateTilePos = new(); // send to EnvironmentManager; invoke at TileManager
        [NonSerialized] public UnityEvent<GameObject, FactionType> OnDomainRegister = new(); // send to EnvironmentManager; invoke at BuildingManager, ResourceManager, CreatureManager

        private void Start()
        {
            StartCoroutine(WaitToStartGame());
        }

        private IEnumerator WaitToStartGame()
        {
            yield return new WaitForSeconds(1f);
            OnLoadData.Invoke();
        }
    }
}
