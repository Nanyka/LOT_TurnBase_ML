using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderManager : AgentManager
{
    public override void KickOffUnitActions()
    {
        m_JumpOverControllers[_responseCounter].AskForAction();

        // Movement cost an amount of point
        m_AgentGroup.AddGroupReward(_movementCost);
        _visualGroupReward += _movementCost;
    }
    
    protected override void EndTurn()
    {
        // call for the end-turn event
        m_Environment.ChangeFaction();
        m_Environment.OnChangeFaction.Invoke();
    }
}
