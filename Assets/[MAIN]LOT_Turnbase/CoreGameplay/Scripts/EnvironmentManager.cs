using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace LOT_Turnbase
{
    [RequireComponent(typeof(MovementInspector))]
    [RequireComponent(typeof(DomainManager))]
    public class EnvironmentManager : MonoBehaviour
    {
        [HideInInspector] public UnityEvent OnChangeFaction; // invoke at JumpOverActuation;
        [HideInInspector] public UnityEvent OnReset; // send to AgentManager
        [HideInInspector] public UnityEvent<int> OnOneTeamWin; // invoke at AgentManager; sent to all AgentManager 
        [HideInInspector] public UnityEvent<Vector3> OnShowMovingPath; // send to MovingPath; invoke at PlayerFactionManager
        [HideInInspector] public UnityEvent<int> OnTouchSelection; // send to PlayerFactionManager; invoke at MovingPath
        [HideInInspector] public UnityEvent<Vector3> OnHighlightUnit; // send to MovingPath; invoke at PlayerFactionManager
        
        [Header("Game Configurations")]
        [SerializeField] protected bool _isObstacleAsTeam1;
        [SerializeField] private bool _isSpawnObstale;
        [SerializeField] protected int _currFaction;
        [SerializeField] protected int _maxStep;
        [SerializeField] protected int _step;

        private DomainManager _domainManager;
        private MovementInspector _movementInspector;

        private void Awake()
        {
            _movementInspector = GetComponent<MovementInspector>();
            _domainManager = GetComponent<DomainManager>();
        }

        protected void Start()
        {
            OnOneTeamWin.AddListener(ResetEnvironment);
            
            KickOffEnvironment();
        }

        #region ENVIRONMENT IN GAME

        protected void ResetEnvironment(int winFaction)
        {
            UIManager.Instance.OnGameOver.Invoke(winFaction);
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

            _currFaction = _currFaction == 0 ? 1 : 0;

            if (_isObstacleAsTeam1)
                _currFaction = 0;
            
            UIManager.Instance.OnRemainStep.Invoke(_maxStep - _step);
        }

        public void ChangeFaction(bool isResetInstance)
        {
            _step++;
            if (_step >= _maxStep)
                OnOneTeamWin.Invoke(1); // if run out of step, NPC win

            _currFaction = _currFaction == 0 ? 1 : 0;

            if (_isObstacleAsTeam1)
                _currFaction = 0;
        }

        private void ResetGame()
        {
            OnReset.Invoke();
            Debug.Log("ResetGame");
        }

        public void KickOffEnvironment()
        {
            if (_isSpawnObstale)
                _domainManager.SpawnObstacle();

            OnChangeFaction.Invoke();
        }

        #endregion

        #region MOVEMENT CALCULATOR

        public MovementInspector GetMovementCalculator()
        {
            return _movementInspector;
        }

        #endregion


        #region OBSTACLES

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

        public void RemoveObject(GameObject targetObject, int faction)
        {
            _domainManager.RemoveObject(targetObject, faction);
            var checkWin = _domainManager.CheckWinCondition();
            if (checkWin >= 0)
                OnOneTeamWin.Invoke(checkWin);
        }

        #endregion

        #region GET

        public int GetCurrFaction()
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