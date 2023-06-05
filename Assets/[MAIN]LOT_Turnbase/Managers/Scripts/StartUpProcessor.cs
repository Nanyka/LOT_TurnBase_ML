using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class StartUpProcessor : Singleton<StartUpProcessor>
    {
        [NonSerialized] public UnityEvent OnLoadData = new(); // send to SavingSystemManager
        [NonSerialized] public UnityEvent<long> OnStartGame = new(); // send to EnvironmentManager
        [NonSerialized] public UnityEvent OnResetData = new(); // sent to EnvironmentLoader
        [NonSerialized] public UnityEvent OnInitiateObjects = new(); // send to Managers; invoke from TileManager
        [NonSerialized] public UnityEvent<Vector3> OnUpdateTilePos = new(); // send to EnvironmentManager; invoke at TileManager
        [NonSerialized] public UnityEvent<GameObject, FactionType> OnDomainRegister = new(); // send to EnvironmentManager; invoke at BuildingManager, ResourceManager, CreatureManager

        private void Start()
        {
            OnLoadData.Invoke();
        }
    }
}
