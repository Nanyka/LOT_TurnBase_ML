using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ParentGoWithRoot : MonoBehaviour
    {
        private Animator _animator;
        private Transform _parentTransform;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _parentTransform = GetParent(transform);
        }

        private void OnAnimatorMove()
        {
            if (_parentTransform == null)
                return;

            // Get root motion values
            Vector3 rootMotion = _animator.deltaPosition;
            Quaternion rootRotation = _animator.deltaRotation;

            // Apply root motion to the parent game object
            _parentTransform.position += rootMotion;
            _parentTransform.rotation *= rootRotation;
        }
        
        private Transform GetParent(Transform upperLevel)
        {
            if (upperLevel.TryGetComponent(out CreatureInGame creatureInGame))
                return upperLevel;
            
            if (upperLevel.parent == null)
                return upperLevel;
            
            upperLevel = upperLevel.parent;
            return GetParent(upperLevel);
        }
    }
}