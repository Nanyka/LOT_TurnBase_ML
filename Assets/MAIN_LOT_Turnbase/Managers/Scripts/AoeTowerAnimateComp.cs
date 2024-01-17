using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class AoeTowerAnimateComp : MonoBehaviour, IAnimateComp
    {
        [SerializeField] private Transform m_RotatePart;

        private Animator m_Animator;
        private bool isReady;
        
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        private static readonly int Attack = Animator.StringToHash("Attack");

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        public void Init(GameObject myGameObject)
        {
            SetRotatePart(myGameObject.transform);
            m_Animator.Rebind();
            Invoke(nameof(TestRebind), 1f);
            isReady = true;
        }

        private void SetRotatePart(Transform rotatePart)
        {
            if (rotatePart.childCount == 0)
                return;

            for (int i = 0; i < rotatePart.childCount; i++)
            {
                var childObject = rotatePart.GetChild(i);
                if (childObject.TryGetComponent(out AoeTowerRotatePart towerRotatePart))
                {
                    m_RotatePart = childObject;
                    break;
                }

                SetRotatePart(childObject);
            }
        }

        private void TestRebind()
        {
            m_Animator.Rebind();
        }

        public void SetAnimation(AnimateType animate)
        {
            throw new System.NotImplementedException();
        }

        public void TriggerAttackAnim(int attackIndex)
        {
            m_Animator.SetInteger(AttackIndex, attackIndex);
            m_Animator.SetTrigger(Attack);
        }

        public void SetBoolValue(string animName, bool value)
        {
            throw new System.NotImplementedException();
        }

        public void SetFloatValue(string param, float value)
        {
            throw new System.NotImplementedException();
        }

        public void SetAnimatorSpeed(float speed)
        {
            throw new System.NotImplementedException();
        }

        public void SetLookAt(Vector3 lookAt)
        {
            if (m_RotatePart != null)
                m_RotatePart.LookAt(lookAt);
        }
    }
}