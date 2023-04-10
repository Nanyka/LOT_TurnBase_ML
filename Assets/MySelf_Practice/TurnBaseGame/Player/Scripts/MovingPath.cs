using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPath : MonoBehaviour
{
    [SerializeField] private EnvironmentInGame m_Environment;
    [SerializeField] protected SelectionCircle[] _movingPoints;
    [SerializeField] private Material[] _attackingColors;
    
    private Camera _camera;
    private int _layerMask = 1 << 8;

    private void Start()
    {
        m_Environment.OnShowMovingPath.AddListener(MovingRange);
        
        _camera = Camera.main;
    }

    private void MovingRange(Vector3 middlePos)
    {
        for (int index = 0; index <= 3; index++)
        {
            _movingPoints[index].SwitchProjector(false);
            
            var currentMovement = m_Environment.GetMovementCalculator()
                .MovingPath(middlePos, index+1, 0, 0); // use "index+1" to show just moving directions
            
            if (Vector3.Distance(currentMovement.returnPos,middlePos) < Mathf.Epsilon)
                continue;
            
            _movingPoints[index].SwitchProjector(currentMovement.returnPos, index+1);
        }
    }

    #region INTERACT SELECTION

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var moveRay = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(moveRay, out var moveHit, 100f, _layerMask))
                return;

            if (!moveHit.collider.TryGetComponent(out SelectionCircle selectionCircle)) return;
            m_Environment.OnTouchSelection.Invoke(selectionCircle.GetDirection());
            DisableMovingPath();
        }
    }

    private void DisableMovingPath()
    {
        foreach (var circle in _movingPoints)
            circle.SwitchProjector(false);
    }

    #endregion
}