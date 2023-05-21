using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
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
            }
        }
    }
}
