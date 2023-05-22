using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LOT_Turnbase
{
    [RequireComponent(typeof(MovementInspector))]
    [RequireComponent(typeof(DomainManager))]
    [RequireComponent(typeof(MovingVisual))]
    public class EnvironmentManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent OnChangeFaction; // invoke at JumpOverActuation;
        [HideInInspector] public UnityEvent OnReset; // send to AgentManager
        [HideInInspector] public UnityEvent<int> OnOneTeamWin; // invoke at AgentManager; sent to all AgentManager 
        [HideInInspector] public UnityEvent<Vector3> OnShowMovingPath; // send to MovingVisual; invoke at FactionController
        [HideInInspector] public UnityEvent<int> OnTouchSelection; // send to PlayerFactionManager; invoke at MovingVisual
        [HideInInspector] public UnityEvent<Vector3> OnHighlightUnit; // send to MovingVisual; invoke at FactionController
        
        [Header("Game Configurations")]
        [SerializeField] protected bool _isObstacleAsTeam1;
        // [SerializeField] private bool _isSpawnObstale;
        [SerializeField] protected FactionType _currFaction;
        [SerializeField] protected int _maxStep;
        [SerializeField] protected int _step;

        private DomainManager _domainManager;
        private MovementInspector _movementInspector;

        private void Awake()
        {
            _movementInspector = GetComponent<MovementInspector>();
            _domainManager = GetComponent<DomainManager>();
            
            OnOneTeamWin.AddListener(ResetEnvironment);
        }

        protected void Start()
        {
            StartUpProcessor.Instance.OnUpdateTilePos.AddListener(UpdateTileArea);
            StartUpProcessor.Instance.OnDomainRegister.AddListener(DomainRegister);
        }

        #region ENVIRONMENT IN GAME
        
        public void KickOffEnvironment()
        {
            // if (_isSpawnObstale)
            //     _domainManager.SpawnObstacle();

            OnChangeFaction.Invoke();
        }
        
        protected void ResetEnvironment(int winFaction)
        {
            MainUI.Instance.OnGameOver.Invoke(winFaction);
            StartCoroutine(WaitToReset());
        }

        private IEnumerator WaitToReset()
        {
            yield return new WaitForSeconds(3f);
            ResetGame();
        }

        public void ChangeFaction()
        {
            _step++;
            if (_step >= _maxStep)
                ResetGame();

            _currFaction = _currFaction == FactionType.Player ? FactionType.Enemy : FactionType.Player;

            if (_isObstacleAsTeam1)
                _currFaction = 0;
            
            MainUI.Instance.OnRemainStep.Invoke(_maxStep - _step);
        }

        public void ChangeFaction(bool isResetInstance)
        {
            _step++;
            if (_step >= _maxStep)
                OnOneTeamWin.Invoke(1); // if run out of step, NPC win

            _currFaction = _currFaction == FactionType.Player ? FactionType.Enemy : FactionType.Player;

            if (_isObstacleAsTeam1)
                _currFaction = 0;
        }

        private void ResetGame()
        {
            OnReset.Invoke();
            Debug.Log("ResetGame");
        }

        #endregion

        #region MOVEMENT CALCULATOR

        public MovementInspector GetMovementCalculator()
        {
            return _movementInspector;
        }

        #endregion
        
        #region OBSTACLES

        public void UpdateTileArea(Vector3 tilePos)
        {
            _domainManager.UpdateTileArea(tilePos);
        }
        
        public void DomainRegister(GameObject domainOwner, FactionType factionType)
        {
            _domainManager.UpdateDomainOwner(domainOwner,factionType);
        }
        
        public bool FreeToMove(Vector3 checkPos)
        {
            return _domainManager.CheckFreeToMove(checkPos);
        }

        public void DestroyObstacle(Vector3 position)
        {
            _domainManager.DestroyAtPosition(position);
        }

        public bool IsRunOutOfObstacle()
        {
            return _domainManager.CountObstacle() == 0;
        }

        public bool CheckObjectInTeam(Vector3 pos, int faction)
        {
            return _domainManager.CheckTeam(pos, faction);
        }

        public bool CheckEnemy(Vector3 pos, int myFaction)
        {
            return _domainManager.CheckEnemy(pos, myFaction);
        }

        public GameObject GetEnemyByPosition(Vector3 position, int fromFaction)
        {
            return _domainManager.GetEnemyByPosition(position, fromFaction);
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