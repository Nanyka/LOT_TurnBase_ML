using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AnimateComp : MonoBehaviour
    {
        [SerializeField] private Animator m_animator;

        public void SetAnimation(AnimateType animate, bool isOn)
        {
            switch (animate)
            {
                case AnimateType.Walk:
                    m_animator.SetBool("Walk",isOn);
                    break;
                case AnimateType.Attack:
                    m_animator.SetTrigger("Attack");
                    break;
            }
        }
    }
}
