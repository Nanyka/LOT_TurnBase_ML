using UnityEngine;

namespace JumpeeIsland
{
    public class AoeZombieAttackComp : MonoBehaviour, IAttackComp
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
            if (target.TryGetComponent(out ICheckableObject checkableObject))
            {
                var myStrength = m_AttackRelated.GetAttackDamage();
                var remainResource = checkableObject.GetRemainAmount();
                var collectedAmount = remainResource > myStrength ? myStrength : remainResource;
                checkableObject.ReduceCheckableAmount(collectedAmount);
                return;
            }
            
            if (target.TryGetComponent(out IHealthComp targetHp))
            {
                targetHp.TakeDamage(m_AttackRelated);
            }
            _mComboMonitor.PowerUp();
            GameFlowManager.Instance.OnMonsterAttack.Invoke();
        }
    }
}