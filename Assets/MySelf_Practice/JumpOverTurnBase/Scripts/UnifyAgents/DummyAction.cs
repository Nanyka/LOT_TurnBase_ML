using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DummyAction
{
    public int AgentIndex;
    public int Action;
    public Vector3 CurrentPos;
    public Vector3 TargetPos;
    public int JumpCount;
    public Vector3 Direction;
    public List<Vector3> JumpOverAt = new(); // limit checking in 1 unit radius around agent
    public int Reward;
    public int VoteAmount;

    public DummyAction(DummyAction dummyAction)
    {
        AgentIndex = dummyAction.AgentIndex;
        Action = dummyAction.Action;
        CurrentPos = dummyAction.CurrentPos;
        TargetPos = dummyAction.TargetPos;
        JumpCount = dummyAction.JumpCount;
        Direction = dummyAction.Direction;
        JumpOverAt = dummyAction.JumpOverAt;
        Reward = dummyAction.Reward;
        VoteAmount = dummyAction.VoteAmount;
    }
    
    public bool CheckTupleExist(int agentIndex, int action)
    {
        return (agentIndex == AgentIndex) && (action == Action);
    }
}