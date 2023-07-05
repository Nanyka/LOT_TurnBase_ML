using System;
using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    [HideInInspector] public UnityEvent<IGetUnitInfo> OnShowUnitInfo; // send to UnitInfoUI; invoke at Units
    [HideInInspector] public UnityEvent<int> OnRemainStep; // send to UIManager; invoke at UnitManagers
    [HideInInspector] public UnityEvent OnClickIdleButton; // send to PlayerFactionManager; invoke at IdleButton & MovingPath
    [HideInInspector] public UnityEvent<int> OnGameOver; // send to GameOverAnnouncer; invoke at PlayerFactionManager
}