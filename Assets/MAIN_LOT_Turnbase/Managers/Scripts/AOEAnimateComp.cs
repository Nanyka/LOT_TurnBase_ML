using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AOEAnimateComp : MonoBehaviour, IAnimateComp, IMover
    {
        [SerializeField] private Transform m_RotatePart;
        
        private Animator m_Animator;
        private bool isReady;

        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Die = Animator.StringToHash("Die");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        public void Init(GameObject skin)
        {
            if (m_RotatePart == null)
                m_RotatePart = transform;

            m_Animator.Rebind();
            Invoke(nameof(TestRebind),1f);
            isReady = true;
        }

        private void TestRebind()
        {
            m_Animator.Rebind();
        }

        public void StartWalk()
        {
            SetAnimation(AnimateType.Walk, true);
        }

        public void StopWalk()
        {
            SetAnimation(AnimateType.Walk, false);
        }

        // Use for trigger animation
        public void SetAnimation(AnimateType animate)
        {
            switch (animate)
            {
                case AnimateType.Die:
                    m_Animator.SetTrigger(Die);
                    break;
                case AnimateType.TakeDamage:
                    m_Animator.SetTrigger(TakeDamage);
                    break;
                case AnimateType.Attack:
                    m_Animator.SetTrigger(Attack);
                    break;
            }
        }

        // Use for bool animation
        private void SetAnimation(AnimateType animate, bool isActivate)
        {
            switch (animate)
            {
                case AnimateType.Walk:
                    m_Animator.SetBool(Walk, isActivate);
                    break;
            }
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
            m_RotatePart.eulerAngles = lookAt;
        }
    }
    
    public interface IMover
    {
        public void StartWalk();
        public void StopWalk();
    }

    public interface IAnimateComp
    {
        public void Init(GameObject myGameObject);
        public void SetAnimation(AnimateType animate);
        public void TriggerAttackAnim(int attackIndex);
        public void SetBoolValue(string animName, bool value);
        public void SetFloatValue(string param, float value);
        public void SetAnimatorSpeed(float speed);
        public void SetLookAt(Vector3 lookAt);
    }
}