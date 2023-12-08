using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeAttackComp : MonoBehaviour, IAttackComp
    {
        private ISkillMonitor m_SkillMonitor;
        private IGetEntityData<CreatureStats> m_Data;

        private void Start()
        {
            m_SkillMonitor = GetComponent<ISkillMonitor>();
            m_Data = GetComponent<IGetEntityData<CreatureStats>>();
        }

        public void SuccessAttack(GameObject target)
        {
            if (target.TryGetComponent(out IHealthComp targetHp))
                targetHp.TakeDamage(m_Data.GetData(), target.GetComponent<IAttackRelated>());
            
            m_SkillMonitor.PowerUp();
        }
    }

    public interface IAttackComp
    {
        public void SuccessAttack(GameObject target);
    }
}