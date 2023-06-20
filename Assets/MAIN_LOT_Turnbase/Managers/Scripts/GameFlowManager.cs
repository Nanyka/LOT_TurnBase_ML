using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class GameFlowManager : Singleton<GameFlowManager>
    {
        [NonSerialized] public UnityEvent OnLoadData = new(); // send to SavingSystemManager
        [NonSerialized] public UnityEvent<long> OnStartGame = new(); // send to EnvironmentManager, invoke at SavingSystemManager
        [NonSerialized] public UnityEvent OnInitiateObjects = new(); // send to Managers; invoke from TileManager
        [NonSerialized] public UnityEvent<Vector3> OnUpdateTilePos = new(); // send to EnvironmentManager; invoke at TileManager
        [NonSerialized] public UnityEvent<GameObject, FactionType> OnDomainRegister = new(); // send to EnvironmentManager; invoke at BuildingManager, ResourceManager, CreatureManager

        public bool IsEcoMode = true;
        private EnvironmentManager _environmentManager;
        
        private void Start()
        {
            _environmentManager = FindObjectOfType<EnvironmentManager>();
            
            OnLoadData.Invoke();
        }

        public EnvironmentManager GetEnvManager()
        {
            return _environmentManager;
        }
    }
}
