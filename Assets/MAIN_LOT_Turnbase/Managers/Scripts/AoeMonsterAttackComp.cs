using UnityEngine;

namespace JumpeeIsland
{
    public class AoeMonsterAttackComp : MonoBehaviour, IAttackComp
    {
        private IComboMonitor _mComboMonitor;
        private IAttackRelated m_AttackRelated;

        private void Start()
        {
            _mComboMonitor = GetComponent<IComboMonitor>();
            m_AttackRelated = GetComponent<IAttackRelated>();
            GetComponent<IGetEntityData<CreatureData>>();
        }

        public void SuccessAttack(GameObject target)
        {
            if (target.TryGetComponent(out IHealthComp targetHp))
                targetHp.TakeDamage(m_AttackRelated);
            _mComboMonitor.PowerUp();
            GameFlowManager.Instance.OnMonsterAttack.Invoke();
        }
    }
}