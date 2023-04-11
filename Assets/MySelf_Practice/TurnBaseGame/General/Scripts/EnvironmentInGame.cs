using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnvironmentInGame : EnvironmentController
{
    [HideInInspector] public UnityEvent<Vector3> OnShowMovingPath; // send to MovingPath; invoke at PlayerFactionManager
    [HideInInspector] public UnityEvent<int> OnTouchSelection; // send to PlayerFactionManager; invoke at MovingPath
    [HideInInspector] public UnityEvent<Vector3> OnHighlightUnit; // send to MovingPath; invoke at PlayerFactionManager
    
    public virtual void ChangeFaction()
    {
        base.ChangeFaction();
        
        UIManager.Instance.OnRemainStep.Invoke(_maxStep - _step);
    }
}