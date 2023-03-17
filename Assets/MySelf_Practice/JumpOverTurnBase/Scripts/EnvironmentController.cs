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
    public UnityEvent<int, int> OnPunishOppositeTeam; // invoke at SingleJumperController; send to AgentManager

    [SerializeField] private ObstacleManager _obstacleManager;
    [SerializeField] private bool _isUseObstacle;
    [SerializeField] private bool _isSpawnObstale;
    [SerializeField] private int _currFaction;
    [SerializeField] private int _maxStep;
    [SerializeField] private int _winJumpAmount;

    [SerializeField] private int _step;


    public void ChangeFaction()
    {
        _step++;
        if (_step == _maxStep)
        {
            _step = 0;
            OnReset.Invoke();
        }

        if (_currFaction == 0)
            _currFaction = 1;
        else
            _currFaction = 0;

        if (_isUseObstacle)
        {
            _currFaction = 0;
            OnChangeFaction.Invoke();
        }
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

    #region Win condition

    // Team will win if reach win amount
    public bool CheckWinCondition(int accumulateJump)
    {
        return accumulateJump >= _winJumpAmount;
    }

    #endregion
}