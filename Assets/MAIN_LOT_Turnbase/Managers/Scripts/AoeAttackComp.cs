using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeAttackComp : MonoBehaviour, IAttackComp
    {
        private IComboMonitor _mComboMonitor;
        private IAttackRelated m_AttackRelated;

        private void Start()
        {
            _mComboMonitor = GetComponent<IComboMonitor>();
            m_AttackRelated = GetComponent<IAttackRelated>();
        }

        public void SuccessAttack(GameObject target)
        {
            if (target.TryGetComponent(out IHealthComp targetHp))
            {
                targetHp.TakeDamage(m_AttackRelated);
                _mComboMonitor.PowerUp();
            }
        }
    }

    public interface IAttackComp
    {
        public void SuccessAttack(GameObject target);
    }
}