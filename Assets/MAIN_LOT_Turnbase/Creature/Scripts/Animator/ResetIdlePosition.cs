using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JumpeeIsland
{
    public class ResetIdlePosition : StateMachineBehaviour
    {
        private Transform m_Transform;
        private Transform _parentTransform;
        
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (m_Transform == null)
                m_Transform = animator.transform;
            
            if (_parentTransform == null)
                _parentTransform = GetParent(m_Transform);

            var position = m_Transform.position;
            _parentTransform.position = new Vector3(Mathf.RoundToInt(position.x),
                Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
        
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

