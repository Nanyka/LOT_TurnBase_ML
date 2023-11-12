using System;
using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;

namespace GOAP
{
    public class EnemyCheckAvailableTarget : GAction
    {
        [SerializeField] private float _checkDistance = 1f;
        [SerializeField] private GameObject[] TestTarget;

        private List<ICheckableObject> _checkableObjects = new();
        private ICheckableObject _currentPoint;

        private void Start()
        {
            foreach (var target in TestTarget)
            {
                if (target.TryGetComponent(out ICheckableObject checkableObject))
                    _checkableObjects.Add(checkableObject);
            }
        }

        public override bool PrePerform()
        {
            var distanceToTarget = float.PositiveInfinity;
            foreach (var target in _checkableObjects)
            {
                if (target.IsCheckable() == false)
                    continue;
                
                var curDis = Vector3.Distance(transform.position, target.GetPosition());
                if (curDis < distanceToTarget)
                {
                    distanceToTarget = curDis;
                    _currentPoint = target;
                }
            }

            if (distanceToTarget < _checkDistance)
            {
                _currentPoint.ReduceCheckableAmount(1);
                m_GAgent.Beliefs.RemoveState("Empty");
                m_GAgent.Beliefs.ModifyState("targetAvailable", 1);
                Duration = 1f;
            }
            else
                Duration = 0f;

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}