using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCSensor : MonoBehaviour
{
    [HideInInspector] public UnityEvent<GameObject> FindItem;
    
    // [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private Transform _viewDirection;

    private Transform _mTransform;
    private bool _isEyeClosed;

    private void Start()
    {
        _mTransform = transform;
        // StartCoroutine(DetectTarget());
    }

    private void Update()
    {
        if (_isEyeClosed)
            return;
        
        var position = _mTransform.position;
        Vector3 targetPos = _viewDirection.position;
        Vector3 aimDir = (targetPos - position).normalized;
        // _fieldOfView.SetAimDirection(aimDir);
        // _fieldOfView.SetOrigin(position);
    }

    // private IEnumerator DetectTarget()
    // {
    //     yield return new WaitUntil(() => _fieldOfView.IsFoundNewTarget());
    //     _fieldOfView.ResetAlarm();
    //     FindItem.Invoke(_fieldOfView.GetTarget());
    //     StartCoroutine(DetectTarget());
    // }
}