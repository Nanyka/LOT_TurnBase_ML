using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DummyAction
{
    public int AgentIndex;
    public int Action;
    public Vector3 TargetPos;
    public int JumpCount;
    public Vector3 Direction;
    public List<Vector3> JumpOverAt = new(); // limit checking in 1 unit radius around agent
    public int Reward;
    public int VoteAmount;

    public bool CheckTupleExist(int agentIndex, int action)
    {
        return agentIndex == AgentIndex && action == Action;
    }
}