using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    [RequireComponent(typeof(MovementInspector))]
    [RequireComponent(typeof(DomainManager))]
    [RequireComponent(typeof(MovingVisual))]
    public class EnvironmentManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent OnChangeFaction; // invoke at EnemyFactionController, PlayerFactionController;
        [HideInInspector] public UnityEvent<FactionType> OnOneTeamWin; // invoke at AgentManager; sent to all AgentManager 
        [HideInInspector] public UnityEvent<Vector3> OnShowMovingPath; // send to MovingVisual; invoke at FactionController
        [HideInInspector] public UnityEvent<int> OnTouchSelection; // send to PlayerFactionManager; invoke at MovingVisual
        [HideInInspector] public UnityEvent<Vector3> OnHighlightUnit; // send to MovingVisual; invoke at FactionController

        [Header("Game Configurations")] [SerializeField]
        protected bool _isObstacleAsTeam1;

        [SerializeField] protected FactionType _currFaction;
        [SerializeField] protected int _minStep;
        [SerializeField] protected int _step;
        [SerializeField] protected float refurbishPeriod;

        private DomainManager _domainManager;
        private MovementInspector _movementInspector;
        private int _lastSessionSteps;
        private bool _isInRefurbish;

        private void Awake()
        {
            _movementInspector = GetComponent<MovementInspector>();
            _domainManager = GetComponent<DomainManager>();

            OnOneTeamWin.AddListener(OneTeamWin);
        }

        private void Start()
        {
            StartUpProcessor.Instance.OnStartGame.AddListener(Init);
            StartUpProcessor.Instance.OnUpdateTilePos.AddListener(UpdateTileArea);
            StartUpProcessor.Instance.OnDomainRegister.AddListener(DomainRegister);
            SavingSystemManager.Instance.OnRestoreCommands.AddListener(CacheLastSessionSteps);
        }

        private void CacheLastSessionSteps()
        {
            Debug.Log("Load command after a disconnected session");
            _lastSessionSteps++;
            SavingSystemManager.Instance.OnUseOneMove.Invoke();
        }

        private void Init(long moveAmount)
        {
            Debug.Log($"Load step from cloud with lasSessionStep: {_lastSessionSteps}");
            _step = (int) moveAmount;
            _step -= _lastSessionSteps;
            MainUI.Instance.OnRemainStep.Invoke(_step);

            // Start refurbish loop
            InvokeRepeating(nameof(WaitToAddMove), refurbishPeriod, refurbishPeriod);
        }

        #region ENVIRONMENT IN GAME

        public void KickOffEnvironment()
        {
            OnChangeFaction.Invoke();
        }

        private void OneTeamWin(FactionType winFaction)
        {
            MainUI.Instance.OnGameOver.Invoke(winFaction);
            Debug.Log("Wait for player claim loot");
        }

        public void ChangeFaction()
        {
            if (_step <= _minStep && _currFaction == FactionType.Player)
            {
                Debug.Log("Run out of steps");
                return;
            }
            
            if (_currFaction == FactionType.Player)
                SpendOneMove();

            _currFaction = _currFaction == FactionType.Player ? FactionType.Enemy : FactionType.Player;

            if (_isObstacleAsTeam1)
                _currFaction = FactionType.Player;
            
            OnChangeFaction.Invoke();
        }

        #endregion

        #region MOVEMENT CALCULATOR

        public MovementInspector GetMovementInspector()
        {
            return _movementInspector;
        }

        private void WaitToAddMove()
        {
            _step++;
            MainUI.Instance.OnRemainStep.Invoke(_step);
        }

        private void SpendOneMove()
        {
            _step--;
            SavingSystemManager.Instance.OnUseOneMove.Invoke();
            MainUI.Instance.OnRemainStep.Invoke(_step);
        }

        #endregion

        #region OBSTACLES

        public void UpdateTileArea(Vector3 tilePos)
        {
            _domainManager.UpdateTileArea(tilePos);
        }

        public void DomainRegister(GameObject domainOwner, FactionType factionType)
        {
            _domainManager.UpdateDomainOwner(domainOwner, factionType);
        }

        public bool FreeToMove(Vector3 checkPos)
        {
            return _domainManager.CheckFreeToMove(checkPos);
        }

        public FactionType CheckFaction(Vector3 objectPos)
        {
            var faction = FactionType.Neutral;
            
            if (_domainManager.CheckTeam(objectPos, FactionType.Player))
                faction = FactionType.Player;
            
            if (_domainManager.CheckTeam(objectPos, FactionType.Enemy))
                faction = FactionType.Enemy;

            return faction;
        }

        public bool CheckEnemy(Vector3 pos, FactionType myFaction)
        {
            return _domainManager.CheckEnemy(pos, myFaction);
        }

        public GameObject GetObjectByPosition(Vector3 position, FactionType fromFaction)
        {
            return _domainManager.GetObjectByPosition(position, fromFaction);
        }

        public void RemoveObject(GameObject targetObject, FactionType faction)
        {
            _domainManager.RemoveObject(targetObject, faction);
            var checkWin = _domainManager.CheckWinCondition();
            if (checkWin >= 0)
                OnOneTeamWin.Invoke(checkWin);
        }

        #endregion

        #region GET

        public FactionType GetCurrFaction()
        {
            return _currFaction;
        }

        // public Collider GetPlatformCollider()
        // {
        //     return _domainManager.GetPlatformCollider();
        // }

        #endregion
    }
}