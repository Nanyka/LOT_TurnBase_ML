using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeAttackComp : MonoBehaviour, IAttackComp
    {
        private ISkillMonitor m_SkillMonitor;
        private IAttackRelated m_AttackRelated;

        private void Start()
        {
            m_SkillMonitor = GetComponent<ISkillMonitor>();
            m_AttackRelated = GetComponent<IAttackRelated>();
        }

        public void SuccessAttack(GameObject target)
        {
            if (target.TryGetComponent(out IHealthComp targetHp))
            {
                targetHp.TakeDamage(m_AttackRelated);
                m_SkillMonitor.PowerUp();
            }
        }
    }

    public interface IAttackComp
    {
        public void SuccessAttack(GameObject target);
    }
}