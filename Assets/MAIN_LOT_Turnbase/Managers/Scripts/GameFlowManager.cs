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
        [NonSerialized] public UnityEvent<EntityData> OnSelectEntity = new(); // send to TutorialController; invoke at PlayerFactionController
        
        public bool IsEcoMode = true;
        public bool _isGameStarted { get; private set; }
        
        private EnvironmentManager _environmentManager;
        private TutorialController _tutorialController;
        
        private void Start()
        {
            _environmentManager = FindObjectOfType<EnvironmentManager>();
            _tutorialController = FindObjectOfType<TutorialController>();
            
            OnStartGame.AddListener(RecordStartedState);
            
            OnLoadData.Invoke();
        }

        private void RecordStartedState(long arg0)
        {
            _isGameStarted = true;
        }

        public EnvironmentManager GetEnvManager()
        {
            return _environmentManager;
        }

        public void LoadCurrentTutorial(string currentTutorial)
        {
            _tutorialController.Init(currentTutorial);
        }
    }
}
