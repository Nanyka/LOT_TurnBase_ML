using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class CameraController : Singleton<CameraController>
    {
        [NonSerialized] public UnityEvent<Vector2> OnMoveFocalPoint = new(); // invoke at InputManager

        [SerializeField] private Transform m_FocalPoint;
        [SerializeField] private float _movingSpeed;
        [SerializeField] private float _lerpSpeed;

        private Vector3 _target;

        private void Start()
        {
            _target = m_FocalPoint.position;
            
            OnMoveFocalPoint.AddListener(MoveFocalPoint);
        }

        private void Update()
        {
            m_FocalPoint.position = Vector3.Lerp(m_FocalPoint.position, _target, _lerpSpeed * Time.deltaTime);
        }

        private void MoveFocalPoint(Vector2 delta)
        {
            delta *= _movingSpeed;
            _target = m_FocalPoint.position;
            _target += new Vector3(delta.x, _target.y, delta.y);
        }
    }
}
