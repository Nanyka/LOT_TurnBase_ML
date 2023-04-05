using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class AgentForInfer : AgentManager
{
    protected override void Start()
    {
        _environmentController.OnChangeFaction.AddListener(ToMyTurn);
        _environmentController.OnReset.AddListener(ResetAgents);
        _environmentController.OnPunishOppositeTeam.AddListener(GetPunish);
        _environmentController.OnOneTeamWin.AddListener(FinishRound);

        MultiJumperKickOff();
    }
    
    protected override void KickOffUnitActions()
    {
        m_JumpOverControllers[_responseCounter].AskForAction();
    }
}
