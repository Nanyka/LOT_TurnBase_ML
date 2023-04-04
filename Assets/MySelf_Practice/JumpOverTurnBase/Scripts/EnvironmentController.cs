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
    [HideInInspector]
    public UnityEvent<int> OnPunishOppositeTeam; // invoke at SingleJumperController; send to AgentManager

    [SerializeField] private ObstacleManager _obstacleManager;
    [SerializeField] private bool _isUseObstacle;
    [SerializeField] private bool _isSpawnObstale;
    [SerializeField] private int _currFaction;
    [SerializeField] private int _maxStep;

    [SerializeField] private int _step;

    private void Start()
    {
        OnOneTeamWin.AddListener(ResetEnvironment);
    }

    private void ResetEnvironment(int winFaction)
    {
        _step = 0;
        if (_isSpawnObstale)
            _obstacleManager.SpawnObstacle();
    }

    public void ChangeFaction()
    {
        _step++;
        if (_step == _maxStep)
        {
            ResetGame();
        }

        if (_currFaction == 0)
            _currFaction = 1;
        else
            _currFaction = 0;

        if (_isUseObstacle)
            _currFaction = 0;
    }

    public void ChangeFaction(bool isResetInstance)
    {
        _step++;
        if (_step == _maxStep)
            ResetGame();
        
        if (isResetInstance)
            ResetGame();
        
        if (_currFaction == 0)
            _currFaction = 1;
        else
            _currFaction = 0;

        if (_isUseObstacle)
            _currFaction = 0;
    }

    public void ResetGame()
    {
        _step = 0;
        OnReset.Invoke();
        ResetEnvironment(0);
    }

    public int GetCurrFaction()
    {
        return _currFaction;
    }

    public void KickOffEnvironment()
    {
        if (_isSpawnObstale)
            _obstacleManager.SpawnObstacle();
        
        OnChangeFaction.Invoke();
    }

    public virtual bool FreeToMove(Vector3 checkPos)
    {
        return !_obstacleManager.CheckObstaclePlot(checkPos, false);
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
}