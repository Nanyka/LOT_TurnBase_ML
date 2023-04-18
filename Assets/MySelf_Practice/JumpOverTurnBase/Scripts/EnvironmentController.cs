using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnvironmentController : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnChangeFaction; // invoke at JumpOverActuation;
    [HideInInspector] public UnityEvent OnReset; // send to AgentManager
    [HideInInspector] public UnityEvent<int> OnOneTeamWin; // invoke at AgentManager; sent to all AgentManager 
    [HideInInspector] public UnityEvent<int> OnPunishOppositeTeam; // invoke at SingleJumperController; send to AgentManager

    [Header("Game Managers")]
    [SerializeField] private ObstacleManager _obstacleManager;
    [SerializeField] private MovementCalculator _movementCalculator;

    [Header("Game Configurations")] 
    [SerializeField] private Collider _platformCollider;
    [SerializeField] protected bool _isObstacleAsTeam1;
    [SerializeField] private bool _isSpawnObstale;
    [SerializeField] protected int _currFaction;
    [SerializeField] protected int _maxStep;
    [SerializeField] protected int _step;

    protected virtual void Start()
    {
        OnOneTeamWin.AddListener(ResetEnvironment);
    }

    private void ResetEnvironment(int winFaction)
    {
        _step = 0;
        if (_isSpawnObstale)
            _obstacleManager.SpawnObstacle();
    }

    public virtual void ChangeFaction()
    {
        _step++;
        if (_step >= _maxStep)
            ResetGame();

        if (_currFaction == 0)
            _currFaction = 1;
        else
            _currFaction = 0;

        if (_isObstacleAsTeam1)
            _currFaction = 0;
    }

    public virtual void ChangeFaction(bool isResetInstance)
    {
        _step++;
        if (_step >= _maxStep)
            ResetGame();

        if (isResetInstance)
            ResetGame();

        if (_currFaction == 0)
            _currFaction = 1;
        else
            _currFaction = 0;

        if (_isObstacleAsTeam1)
            _currFaction = 0;
    }

    private void ResetGame()
    {
        OnReset.Invoke();
        ResetEnvironment(0);
    }

    public void KickOffEnvironment()
    {
        if (_isSpawnObstale)
            _obstacleManager.SpawnObstacle();

        OnChangeFaction.Invoke();
    }
    
    #region MOVEMENT CALCULATOR

    public MovementCalculator GetMovementCalculator()
    {
        return _movementCalculator;
    }

    #endregion
    
    
    #region OBSTACLES

    public bool FreeToMove(Vector3 checkPos)
    {
        return !_obstacleManager.CheckObstaclePlot(checkPos);
    }

    public void DestroyObstacle(Vector3 position)
    {
        _obstacleManager.DestroyAtPosition(position);
    }

    public bool IsRunOutOfObstacle()
    {
        return _obstacleManager.CountObstacle() == 0;
    }
    
    public bool CheckObjectInTeam(Vector3 pos, int faction)
    {
        return _obstacleManager.CheckTeam(pos, faction);
    }

    public bool CheckEnemy(Vector3 pos, int myFaction)
    {
        return _obstacleManager.CheckEnemy(pos, myFaction);
    }

    public GameObject GetEnemyByPosition(Vector3 position, int fromFaction)
    {
        return _obstacleManager.GetEnemyByPosition(position, fromFaction);
    }

    public void RemoveObject(GameObject targetObject, int faction)
    {
        _obstacleManager.RemoveObject(targetObject,faction);
        var checkWin = _obstacleManager.CheckWinCondition();
        if (checkWin >=0)
            OnOneTeamWin.Invoke(checkWin);
    }

    #endregion

    #region GET

    public int GetCurrFaction()
    {
        return _currFaction;
    }

    public Collider GetPlatformCollider()
    {
        return _platformCollider;
    }
    
    #endregion
}