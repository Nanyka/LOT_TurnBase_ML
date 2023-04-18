using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnvironmentInGame : EnvironmentController
{
    [HideInInspector] public UnityEvent<Vector3> OnShowMovingPath; // send to MovingPath; invoke at PlayerFactionManager
    [HideInInspector] public UnityEvent<int> OnTouchSelection; // send to PlayerFactionManager; invoke at MovingPath
    [HideInInspector] public UnityEvent<Vector3> OnHighlightUnit; // send to MovingPath; invoke at PlayerFactionManager
    
    public override void ChangeFaction(bool isResetInstance)
    {
        _step++;
        if (_step >= _maxStep)
            OnOneTeamWin.Invoke(1); // if run out of step, NPC win

        if (_currFaction == 0)
            _currFaction = 1;
        else
            _currFaction = 0;

        if (_isObstacleAsTeam1)
            _currFaction = 0;
    }
    
    public override void ChangeFaction()
    {
        base.ChangeFaction();
        
        UIManager.Instance.OnRemainStep.Invoke(_maxStep - _step);
    }
}