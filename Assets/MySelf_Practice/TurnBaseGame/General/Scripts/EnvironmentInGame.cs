using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnvironmentInGame : EnvironmentController
{
    [HideInInspector] public UnityEvent<Vector3> OnShowMovingPath; // send to MovingPath; invoke at PlayerFactionManager
    [HideInInspector] public UnityEvent<int> OnTouchSelection; // send to PlayerFactionManager; invoke at MovingPath

    public override void ChangeFaction()
    {
        if (_currFaction == 0)
            _currFaction = 1;
        else
            _currFaction = 0;

        if (_isUseObstacle)
            _currFaction = 0;
    }
}