using System;using System.Collections.Generic;using System.Linq;using UnityEngine;using UnityEngine.Serialization;using WebSocketSharp;namespace JumpeeIsland{    public class TutorialController : MonoBehaviour    {        [SerializeField] private Tutorial m_CustomizedTutorial;        [SerializeField] private bool _isNotUseTutorial;        [FormerlySerializedAs("m_Pointer")] [Header("Tutorial tools")] [SerializeField]        private ParticleSystem[] m_Pointers;        [SerializeField] private Tutorial _currentTutorial;        private EnvironmentManager _environmentManager;        [SerializeField] private int _currentStepIndex;        private TutorialStep _currentStep;        private EntityData _waitingForEntity;        private Vector3 _waitingForPosition;        private GameObject _waitingUIElement;        private Camera _camera;        private int _platformLayer = 1 << 6;        [SerializeField] private bool _isTutorialCompleted;        private void Start()        {            _environmentManager = FindObjectOfType<EnvironmentManager>();            _environmentManager.OnChangeFaction.AddListener(StartTutorial);            GameFlowManager.Instance.OnSelectEntity.AddListener(DetectEntity);            GameFlowManager.Instance.OnStartGame.AddListener(FirstSession);            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(CheckStepCondition);            MainUI.Instance.OnHideAllMenu.AddListener(MarkStepCompleted);            MainUI.Instance.OnBuyBuildingMenu.AddListener(WhenOpenMenus);            MainUI.Instance.OnShowCreatureMenu.AddListener(WhenOpenMenus);            MainUI.Instance.OnShowAnUI.AddListener(MarkStepCompleted);            MainUI.Instance.OnShowBossSelector.AddListener(MarkStepCompleted);            _camera = Camera.main;        }        public void Init(string currentTutorial)        {            if (currentTutorial.IsNullOrEmpty())            {                _isNotUseTutorial = true;                return;            }                        if (currentTutorial.Equals("MAX"))                return;            if (m_CustomizedTutorial != null)                LoadTutorial(m_CustomizedTutorial);            else            {                if (GameFlowManager.Instance.GameMode == GameMode.BOSS)                    LoadTutorialFromQuest(currentTutorial);                else                    LoadTutorial(currentTutorial);            }        }        private void FirstSession(long arg0)        {            StartTutorial();        }        private void WhenOpenMenus(List<JIInventoryItem> arg0)        {            MarkStepCompleted();        }        private void MarkStepCompleted()        {            if (_currentTutorial == null || _currentStep == null)                return;            if (_isTutorialCompleted)            {                return;            }            if (_currentStep.CheckPosition)            {                var moveRay = _camera.ScreenPointToRay(Input.mousePosition);                if (!Physics.Raycast(moveRay, out var moveHit, 100f, _platformLayer))                    return;                var pos = moveHit.transform.position;                if (Vector3.Distance(_waitingForPosition, pos) < 0.1f)                    StepCompletedDecide();            }            if (_currentStep.CheckEndCondition)            {                if (_currentStep.EndCondition.CheckPass())                {                    if (_currentStep.IsEndTutorial)                        TutorialCompletedDecide();                    else                        StepCompletedDecide();                }            }            if (_currentStep.ButtonSelection)            {                if (_waitingUIElement != null)                    MainUI.Instance.OnSwitchButtonPointer.Invoke(_waitingUIElement.transform.position,                        _waitingUIElement.activeInHierarchy);            }        }        private void LoadTutorial(string tutorialAddress)        {            if (m_CustomizedTutorial != null)                return;            if (tutorialAddress.IsNullOrEmpty())            {                _currentTutorial = null;                SavingSystemManager.Instance.SaveCurrentTutorial("MAX");                return;            }            Debug.Log("Load new tutorial and update JI_GAME_PROCESS on cloud");            SavingSystemManager.Instance.SaveCurrentTutorial(tutorialAddress);            if (_currentTutorial != null)                SavingSystemManager.Instance.SendTutorialTrackEvent(_currentTutorial.name);            _currentTutorial = AddressableManager.Instance.GetAddressableSO(tutorialAddress) as Tutorial;        }        // Just for TESTING        private void LoadTutorial(Tutorial tutorial)        {            _currentTutorial = tutorial;        }        // When running tutorial in BOSS mode, just use customized tutorial but not on-cloud tutorial        private void LoadTutorialFromQuest(string tutorialAddress)        {            m_CustomizedTutorial = AddressableManager.Instance.GetAddressableSO(tutorialAddress) as Tutorial;            _currentTutorial = m_CustomizedTutorial;        }        #region TUTORIAL PROCESS        private void StartTutorial()        {            if (_isNotUseTutorial)                return;            // if (GameFlowManager.Instance.GameMode == GameMode.ECONOMY &&            //     GameFlowManager.Instance._isGameRunning == false)            //     return;            if (_environmentManager.GetCurrFaction() == FactionType.Enemy)                return;            _currentStepIndex = 0;            if (_isTutorialCompleted)            {                _isTutorialCompleted = false;                HideAllTool();                if (_currentTutorial == null)                    return;                _isTutorialCompleted = false;                LoadTutorial(_currentTutorial.GetNextTutorial());            }            if (_currentTutorial == null)                return;            if (_currentTutorial.CheckPassCondition())            {                TutorialCompletedDecide();                return;            }            // Check condition and execute tutorial            if (_currentTutorial.CheckExecute() == false)                return;            ExecuteStep();        }        private void ExecuteStep()        {            HideAllTool();            _waitingForEntity = null;            _currentStep = _currentTutorial.GetStep(_currentStepIndex);            if (_currentStep.Pointer)            {                if (_currentStep.EntitySelection)                {                    switch (_currentStep.EntityType)                    {                        case EntityType.PLAYER:                            SelectPlayer();                            break;                        case EntityType.RESOURCE:                            SelectResource();                            break;                        case EntityType.COLLECTABLE:                            SelectCollectable();                            break;                        case EntityType.BUILDING:                            SelectBuilding();                            break;                        case EntityType.ENEMY:                            SelectEnemy();                            break;                    }                }                else if (_currentStep.ButtonSelection)                {                    ShowButtonPointer();                }            }            if (_currentStep.Spawner)                SpawnHintObject();            ShowConversationDialog(_currentStep.Message, _currentStep.Conversation);        }        private void SelectPlayer()        {            var playerCreature = SavingSystemManager.Instance.GetEnvironmentData().PlayerData[0];            if (_currentStep.CheckEntity)                _waitingForEntity = playerCreature;            ShowPointerAtPlayer(playerCreature);        }        private void SelectCollectable()        {            var collectables = SavingSystemManager.Instance.GetEnvironmentData().CollectableData;            if (collectables.Count == 0)                return;            var collectable = collectables[0];            if (_currentStep.CheckEntity)                _waitingForEntity = collectable;            ShowPointerAtEntity(collectable, EntityType.COLLECTABLE);        }        private void SelectResource()        {            var resources = SavingSystemManager.Instance.GetEnvironmentData().ResourceData;            if (resources.Count == 0)                return;            var resource = resources.Find(t => t.CollectedCurrency == _currentStep.CurrencyFromResource);            if (resource == null)                return;            if (_currentStep.CheckEntity)                _waitingForEntity = resource;            ShowPointerAtEntity(resource, EntityType.RESOURCE);        }        private void SelectBuilding()        {            var buildings = SavingSystemManager.Instance.GetEnvironmentData().BuildingData;            if (buildings.Count == 0)                return;            var building = buildings.Find(t => t.BuildingType == _currentStep.BuildingType);            if (building == null)                return;            if (_currentStep.CheckEntity)                _waitingForEntity = building;            ShowPointerAtEntity(building, EntityType.BUILDING);        }        private void SelectEnemy()        {            var enemies = SavingSystemManager.Instance.GetEnvironmentData().EnemyData;            if (enemies.Count == 0)                return;            var enemy = enemies.Find(t => t.CreatureType == _currentStep.CreatureType);            if (enemy == null)                return;            if (_currentStep.CheckEntity)                _waitingForEntity = enemy;            ShowPointerAtEntity(enemy, EntityType.ENEMY);        }        private void DetectEntity(EntityData entityData)        {            if (_currentStep == null)                return;            if (_currentStep.CheckPosition || _currentStep.CheckEntity == false)                return;            if (_waitingForEntity == entityData)                StepCompletedDecide();        }        private void StepCompletedDecide()        {            // Move to the next step if current step completed            _currentStepIndex++;            if (_currentTutorial.CheckRunOutOfStep(_currentStepIndex))            {                _isTutorialCompleted = true;                HideAllTool();            }            else                ExecuteStep();        }        private void TutorialCompletedDecide()        {            _isTutorialCompleted = true;            HideAllTool();            StartTutorial();        }        private void CheckStepCondition(IRemoveEntity removeEntity)        {            if (_currentStep == null)                return;            if (_currentStep.CheckEndCondition && _currentStep.EndCondition.CheckPass())                _isTutorialCompleted = true;        }        private void CheckStepCondition()        {            if (_currentStep == null)                return;            if (_currentStep.CheckEndCondition && _currentStep.EndCondition.CheckPass())                _isTutorialCompleted = true;        }        #endregion        #region TUTORIAL TOOLS        private void ShowPointerAtPlayer(CreatureData creatureData)        {            var position = creatureData.Position;            if (_currentStep.ArrowSign)            {                foreach (var direction in Enumerable.Range(1, 4))                {                    var movement = GameFlowManager.Instance.GetEnvManager().GetMovementInspector()                        .MovingPath(creatureData.Position, direction, 0, 0);                    if (movement.jumpCount >= _currentStep.MinJump)                    {                        position = movement.returnPos;                        break;                    }                }            }            if (_currentStep.CheckPosition)            {                if (Vector3.Distance(position, creatureData.Position) < 0.1f)                {                    HideAllTool();                    return;                }                _waitingForPosition = position;            }            PlacePointer(position, EntityType.PLAYER);        }        private void ShowPointerAtEntity(EntityData entityData, EntityType entityType)        {            var position = entityData.Position;            _waitingForPosition = position;            PlacePointer(_waitingForPosition, entityType);        }        private void PlacePointer(Vector3 position, EntityType entityType)        {            // if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out var pointHit, 100f))            //     position = pointHit.point;            switch (entityType)            {                case EntityType.PLAYER:                {                    m_Pointers[0].transform.position = position;                    m_Pointers[0].Play();                }                    break;                case EntityType.BUILDING:                {                    m_Pointers[0].transform.position = position;                    m_Pointers[0].Play();                }                    break;                case EntityType.COLLECTABLE:                {                    m_Pointers[1].transform.position = position;                    m_Pointers[1].Play();                }                    break;                case EntityType.RESOURCE:                {                    m_Pointers[2].transform.position = position;                    m_Pointers[2].Play();                }                    break;                case EntityType.ENEMY:                {                    m_Pointers[2].transform.position = position;                    m_Pointers[2].Play();                }                    break;            }            // m_Pointer.transform.position = position;            // m_Pointer.SetActive(true);        }        private void ShowButtonPointer()        {            _waitingUIElement = MainUI.Instance.GetButtonPosition(_currentStep.ButtonName);            if (_waitingUIElement != null)                MainUI.Instance.OnSwitchButtonPointer.Invoke(_waitingUIElement.transform.position, true);        }        private void SpawnHintObject()        {            var availableTile = GameFlowManager.Instance.GetEnvManager().GetPotentialTile();            if (_currentStep.IsDesignatedPos)                availableTile = _currentStep.DesignatedPos;            if (availableTile != Vector3.negativeInfinity)            {                switch (_currentStep.SpawnType)                {                    case EntityType.RESOURCE:                        SavingSystemManager.Instance.OnSpawnResource(_currentStep.SpawnResource, availableTile);                        break;                    case EntityType.COLLECTABLE:                        SavingSystemManager.Instance.OnSpawnCollectable(_currentStep.SpawnCollectable, availableTile,                            _currentStep.SpawnCollectableLevel);                        break;                    case EntityType.ENEMY:                        SavingSystemManager.Instance.OnSpawnMovableEntity(_currentStep.SpawnEnemy, availableTile);                        break;                }                StepCompletedDecide();            }        }        private void ShowConversationDialog(string message, bool showDialog)        {            MainUI.Instance.OnConversationUI.Invoke(message, showDialog);        }        private void HideAllTool()        {            // foreach (var pointer in m_Pointers)            //     pointer.SetActive(false);            MainUI.Instance.OnSwitchButtonPointer.Invoke(Vector3.zero, false);            MainUI.Instance.OnConversationUI.Invoke("", false);        }        #endregion    }}