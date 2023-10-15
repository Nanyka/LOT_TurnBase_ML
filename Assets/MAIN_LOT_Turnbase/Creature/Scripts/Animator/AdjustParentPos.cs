using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AdjustParentPos : StateMachineBehaviour
    {
        [SerializeField] private Transform _parentTransform;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_parentTransform == null)
                _parentTransform = GetParent(animator.transform);
        }
        
        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Get root motion values
            Vector3 rootMotion = animator.deltaPosition;
        
            // Apply root motion to the parent game object
            _parentTransform.position += rootMotion;
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