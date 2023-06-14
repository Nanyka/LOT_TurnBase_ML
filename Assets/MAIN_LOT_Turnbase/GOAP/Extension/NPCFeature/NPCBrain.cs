using System.Collections.Generic;
using GOAP;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCBrain : GAgent
{
    [SerializeField] private NPCGoalManager _mGoalManager;
    [SerializeField] private NPCSensor mNpcSensor;
    [SerializeField] private NPCBlackboard _mBlackBoard;

    private bool _isBusy;

    // private void OnEnable()
    // {
    //     mNpcSensor.FindItem.AddListener(DetectTarget);
    // }
    //
    // private void OnDisable()
    // {
    //     mNpcSensor.FindItem.RemoveListener(DetectTarget);
    // }

    protected override void Start()
    {
        base.Start();
        SetGoal();
    }

    #region GOAL MANAGER

    private void SetGoal()
    {
        var subGoals = _mGoalManager.GetCurrentGoal();
        foreach (var goal in subGoals)
            Goals.Add(goal,goal.Weight);
        
        Invoke("GetIdle", Random.Range(3,5));
    }
    
    private void GetIdle()
    {
        Beliefs.ModifyState("idle", 0);
        Invoke("GetIdle", Random.Range(3,5));
    }

    #endregion

    #region SENSORs

    // private void DetectTarget(GameObject target)
    // {
    //     if (_isBusy)
    //     {
    //         if (Inventory.IsEmpty())
    //             _isBusy = false;
    //         return;
    //     }
    //     
    //     if (target.CompareTag("Item"))
    //     {
    //         Beliefs.ModifyState("foundAnItem",1);
    //         Inventory.AddItem(target);
    //         _isBusy = true;
    //     }
    // }

    #endregion

    #region BLACKBOARD

    public NPCBlackboard GetBlackBoard()
    {
        return _mBlackBoard;
    }
    
    public void RememberPos(Vector3 position)
    {
        _mBlackBoard.SetTargetPos(position);
    }

    #endregion
}
