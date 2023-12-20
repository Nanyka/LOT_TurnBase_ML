using UnityEngine;

namespace JumpeeIsland
{
    public class AoeMonsterAttackComp : MonoBehaviour, IAttackComp
    {
        private IComboMonitor _mComboMonitor;
        private IAttackRelated m_AttackRelated;
        private IGetEntityData<CreatureData> m_Data;

        private void Start()
        {
            _mComboMonitor = GetComponent<IComboMonitor>();
            m_AttackRelated = GetComponent<IAttackRelated>();
            m_Data = GetComponent<IGetEntityData<CreatureData>>();
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