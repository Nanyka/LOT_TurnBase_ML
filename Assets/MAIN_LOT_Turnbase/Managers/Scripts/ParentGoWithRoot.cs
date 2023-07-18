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
            _parentTransform = transform.parent;
        }
        
        private void OnAnimatorMove()
        {
            // Get root motion values
            Vector3 rootMotion = _animator.deltaPosition;
            Quaternion rootRotation = _animator.deltaRotation;

            // Apply root motion to the parent game object
            _parentTransform.position += rootMotion;
            _parentTransform.rotation *= rootRotation;
        }
    }
}
