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
        [HideInInspector] public UnityEvent OnChangeFaction; // invoke at EnemyFactionController, PlayerFactionController;
        // [HideInInspector] public UnityEvent OnOneTeamZeroTroop; // sent to FactionManagers, MainUI 
        [HideInInspector] public UnityEvent<Vector3> OnShowMovingPath; // send to MovingVisual; invoke at FactionController
        [HideInInspector] public UnityEvent<int> OnTouchSelection; // send to PlayerFactionManager; invoke at MovingVisual
        [HideInInspector] public UnityEvent<Vector3> OnHighlightUnit; // send to MovingVisual; invoke at FactionController

        [Header("Game Configurations")]
        [SerializeField] protected bool _isObstacleAsTeam1;
        [SerializeField] protected FactionType _currFaction = FactionType.Player;
        [SerializeField] protected int _minStep;
        [SerializeField] protected int _step;
        [SerializeField] protected float _refurbishPeriod;

        private DomainManager _domainManager;
        private MovementInspector _movementInspector;
        private int _lastSessionSteps;
        private bool _isInRefurbish;

        private void Awake()
        {
            _movementInspector = GetComponent<MovementInspector>();
            _domainManager = GetComponent<DomainManager>();
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
            _step = (int) moveAmount;
            
            // Start refurbish loop
            InvokeRepeating(nameof(WaitToAddMove), _refurbishPeriod, _refurbishPeriod);
        }

        #region ENVIRONMENT IN GAME

        private void KickOffEnvironment()
        {
            OnChangeFaction.Invoke();
        }

        public void ChangeFaction()
        {
            // Just use MOVE currency in EcoMode
            if (GameFlowManager.Instance.IsEcoMode)
            {
                if (_step <= _minStep && _currFaction == FactionType.Player)
                {
                    Debug.Log("Show Run out of steps panel");
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
            _step++;
            MainUI.Instance.OnRemainStep.Invoke(_step);
        }

        private void SpendOneMove()
        {
            _step--;
            SavingSystemManager.Instance.OnContributeCommand.Invoke(CommandName.JI_SPEND_MOVE);
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

        public Vector3 GetPotentialTile()
        {
            return _domainManager.GetPotentialTile();
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
            // if (GameFlowManager.Instance.IsEcoMode == false)
            // {
            //     if (_domainManager.CheckOneFactionZeroTroop())
            //         MainUI.Instance.OnGameOver.Invoke();
            // }
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