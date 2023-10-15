using System;
using UnityEngine;
using UnityEngine.Animations;

namespace JumpeeIsland
{
    public class PositionComp : MonoBehaviour
    {
        [SerializeField] private ParentConstraint _parentConstraint;

        private void Start()
        {
            var source = new ConstraintSource();
            source.sourceTransform = Camera.main.transform;
            source.weight = 1;
            _parentConstraint.AddSource(source);
        }
    }
}