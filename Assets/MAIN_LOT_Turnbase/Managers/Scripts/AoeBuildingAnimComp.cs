using UnityEngine;

namespace JumpeeIsland
{
    public class AoeBuildingAnimComp : MonoBehaviour, IAnimateComp
    {
        private Animator m_Animator;
        private bool isReady;
        
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");

        public void Init(GameObject myGameObject)
        {
            m_Animator = GetComponent<Animator>();
            isReady = true;
        }

        public void SetAnimation(AnimateType animate)
        {
            
        }

        public void SetAnimation(AnimateType animate, bool isActivate)
        {
            
        }

        public void TriggerAttackAnim(int attackIndex)
        {
            m_Animator.SetInteger(AttackIndex, attackIndex);
            m_Animator.SetTrigger(Attack);
        }

        public void SetBoolValue(string animName, bool value)
        {
            m_Animator.SetBool(animName,value);
        }

        public void SetFloatValue(string param, float value)
        {
            m_Animator.SetFloat(param,value);
        }

        public void SetAnimatorSpeed(float speed)
        {
            m_Animator.speed = speed;
        }

        public void SetLookAt(Vector3 lookAt)
        {
            
        }
    }
}