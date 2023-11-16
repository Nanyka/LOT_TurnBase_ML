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

        private void Start()
        {
            OnMoveFocalPoint.AddListener(MoveFocalPoint);
        }

        private void MoveFocalPoint(Vector2 delta)
        {
            delta *= _movingSpeed;
            var position = m_FocalPoint.position;
            position += new Vector3(delta.x, position.y, delta.y);
            m_FocalPoint.position = position;
        }
    }
}
