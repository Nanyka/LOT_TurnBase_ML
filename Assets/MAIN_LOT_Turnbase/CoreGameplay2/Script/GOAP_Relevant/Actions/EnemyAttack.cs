using System;
using System.Collections.Generic;
using GOAP;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class EnemyAttack : GAction, ICharacterAttack
    {
        [SerializeField] private float _checkDistance = 1f;
        [SerializeField] private string _targetTag;
        [SerializeField] private CharacterEntity _character;
        // [SerializeField] private GameObject[] TestTarget;

        // private List<ICheckableObject> _checkableObjects = new();
        // private ICheckableObject _currentPoint;

        // private void Start()
        // {
        //     foreach (var target in TestTarget)
        //     {
        //         if (target.TryGetComponent(out ICheckableObject checkableObject))
        //             _checkableObjects.Add(checkableObject);
        //     }
        // }

        public override bool PrePerform()
        {
            // var distanceToTarget = float.PositiveInfinity;
            // foreach (var target in _checkableObjects)
            // {
            //     if (target.IsCheckable() == false)
            //         continue;
            //     
            //     var curDis = Vector3.Distance(transform.position, target.GetPosition());
            //     if (curDis < distanceToTarget)
            //     {
            //         distanceToTarget = curDis;
            //         _currentPoint = target;
            //     }
            // }

            var target = m_GAgent.Inventory.FindItemWithTag(_targetTag);
            if (target == null)
            {
                Duration = 0f;
                return true;
            }

            if (target.TryGetComponent(out ICheckableObject checkableObject))
            {
                if (Vector3.Distance(transform.position,checkableObject.GetPosition()) < _checkDistance && checkableObject.IsCheckable())
                {
                    _character.StartAttack(this);
                    m_GAgent.Beliefs.RemoveState("Empty");
                    m_GAgent.Beliefs.ModifyState("targetAvailable", 1);
                    Duration = 1f;
                }
                else
                    Duration = 0f;
            }

            return true;
        }

        public override bool PostPerform()
        {
            m_GAgent.Inventory.ClearInventory();
            return true;
        }

        public void ExecuteAttack(GameObject target)
        {
            if (target.TryGetComponent(out ICheckableObject checkableObject))
                checkableObject.ReduceCheckableAmount(1);
        }
    }
}