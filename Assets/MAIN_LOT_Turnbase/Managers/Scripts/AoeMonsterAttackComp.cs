using UnityEngine;

namespace JumpeeIsland
{
    public class AoeMonsterAttackComp : MonoBehaviour, IAttackComp
    {
        private ISkillMonitor m_SkillMonitor;
        private IAttackRelated m_AttackRelated;
        private IGetEntityData<CreatureData> m_Data;

        private void Start()
        {
            m_SkillMonitor = GetComponent<ISkillMonitor>();
            m_AttackRelated = GetComponent<IAttackRelated>();
            m_Data = GetComponent<IGetEntityData<CreatureData>>();
        }

        public void SuccessAttack(GameObject target)
        {
            if (target.TryGetComponent(out IHealthComp targetHp))
                targetHp.TakeDamage(m_AttackRelated);
            m_SkillMonitor.PowerUp();
            // SavingSystemManager.Instance.GetMonsterController().AddMonster(m_Data);
            GameFlowManager.Instance.OnMonsterAttack.Invoke();
            // TODO: connect player troop together and share information when one is under attack
        }
    }
}