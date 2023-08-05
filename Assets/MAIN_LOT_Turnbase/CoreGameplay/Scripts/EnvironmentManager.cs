using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [RequireComponent(typeof(MovementInspector))]
    [RequireComponent(typeof(DomainManager))]
    [RequireComponent(typeof(MovingVisual))]
    public class EnvironmentManager : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent OnChangeFaction; // invoke at EnemyFactionController, PlayerFactionController;

        // [HideInInspector] public UnityEvent OnOneTeamZeroTroop; // sent to FactionManagers, MainUI 
        [HideInInspector]
        public UnityEvent<Vector3> OnShowMovingPath; // send to MovingVisual; invoke at FactionController

        [HideInInspector]
        public UnityEvent<int> OnTouchSelection; // send to PlayerFactionManager; invoke at MovingVisual

        [HideInInspector]
        public UnityEvent<Vector3> OnHighlightUnit; // send to MovingVisual; invoke at FactionController

        [Header("Game Configurations")] [SerializeField]
        protected bool _isObstacleAsTeam1;

        [SerializeField] protected FactionType _currFaction = FactionType.Player;
        [SerializeField] protected int _minStep;
        [SerializeField] protected int _step;
        [SerializeField] protected float _refurbishPeriod;

        private DomainManager _domainManager;
        private MovementInspector _movementInspector;
        private MovingVisual _movingVisual;
        private int _lastSessionSteps;
        private bool _isInRefurbish;
        private bool _isRunOutOfStep;

        private void Awake()
        {
            _movementInspector = GetComponent<MovementInspector>();
            _domainManager = GetComponent<DomainManager>();
            _movingVisual = GetComponent<MovingVisual>();
        }

        private void Start()
        {
            GameFlowManager.Instance.OnStartGame.AddListener(Init);
            GameFlowManager.Instance.OnUpdateTilePos.AddListener(UpdateTileArea);
            GameFlowManager.Instance.OnDomainRegister.AddListener(DomainRegister);
            GameFlowManager.Instance.OnKickOffEnv.AddListener(KickOffEnvironment); // Just for BATTLE MODE
        }

        private void Init(long moveAmount)
        {
            _step = (int)moveAmount;
            OnChangeFaction.Invoke();

            // Start refurbish loop
            InvokeRepeating(nameof(WaitToAddMove), _refurbishPeriod, _refurbishPeriod);
        }

        public void UpdateRemainStep(int remainStep)
        {
            _step = remainStep;
            if (_isRunOutOfStep)
            {
                OnChangeFaction.Invoke();
                _isRunOutOfStep = false;
            }
        }

        #region ENVIRONMENT IN GAME

        private void KickOffEnvironment()
        {
            if (GameFlowManager.Instance.IsEcoMode == false)
                OnChangeFaction.Invoke();
        }

        public void ChangeFaction()
        {
            // Just use MOVE currency in EcoMode
            if (GameFlowManager.Instance.IsEcoMode)
            {
                if (_step <= _minStep && _currFaction == FactionType.Player)
                {
                    _movingVisual.DisableMovingPath();
                    MainUI.Instance.OnConversationUI.Invoke("Run out of steps", true);
                    MainUI.Instance.OnUpdateCurrencies.Invoke();
                    _isRunOutOfStep = true;
                    return;
                }

                if (_currFaction == FactionType.Enemy)
                    SpendOneMove();
            }

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
            if (GameFlowManager.Instance.IsEcoMode)
            {
                Debug.Log($"Grant one move");
                _step++;
                MainUI.Instance.OnRemainStep.Invoke(_step);
            }
        }

        private void SpendOneMove()
        {
            _step--;
            SavingSystemManager.Instance.OnContributeCommand.Invoke(CommandName.JI_SPEND_MOVE);
        }

        public bool CheckTileHeight(Vector3 geoPos1, Vector3 geoPos2)
        {
            return _domainManager.CheckTilesHeight(geoPos1, geoPos2);
        }

        public bool CheckHigherTile(Vector3 curTile, Vector3 checkTile)
        {
            return _domainManager.CheckHigherTile(curTile, checkTile);
        }

        public Vector3 GetTilePosByGeoPos(Vector3 geoPos)
        {
            var tile = _domainManager.GetTileByGeoCoordinates(geoPos);
            if (tile == null)
                return Vector3.negativeInfinity;
            return _domainManager.GetTileByGeoCoordinates(geoPos).GetPosition();
        }

        #endregion

        #region OBSTACLES

        private void UpdateTileArea(MovableTile tilePos)
        {
            _domainManager.UpdateTileArea(tilePos);
        }

        private void DomainRegister(GameObject domainOwner, FactionType factionType)
        {
            _domainManager.UpdateDomainOwner(domainOwner, factionType);
        }

        public Vector3 GetPotentialTile()
        {
            return _domainManager.GetPotentialTile();
        }

        public Vector3 GetRandomAvailableTile()
        {
            return _domainManager.GetAvailableTile();
        }

        public bool FreeToMove(Vector3 checkPos)
        {
            return _domainManager.CheckFreeToMove(checkPos);
        }

        public bool CheckOutOfBoundary(Vector3 checkPos)
        {
            return !_domainManager.CheckTileExist(checkPos);
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

        public bool CheckAlly(Vector3 pos, FactionType myFaction)
        {
            return _domainManager.CheckAlly(pos, myFaction);
        }

        public GameObject GetObjectByPosition(Vector3 position, FactionType fromFaction)
        {
            return _domainManager.GetObjectByPosition(position, fromFaction);
        }

        public void RemoveObject(GameObject targetObject, FactionType faction)
        {
            _domainManager.RemoveObject(targetObject, faction);
        }

        #endregion

        #region GET

        public FactionType GetCurrFaction()
        {
            return _currFaction;
        }

        public int CountFaction(FactionType factionType)
        {
            return _domainManager.CountFaction(factionType);
        }

        #endregion
    }
}