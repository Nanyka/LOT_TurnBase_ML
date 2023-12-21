using UnityEngine;

namespace JumpeeIsland
{
    public class AoeIceTowerAttackComp : MonoBehaviour, IAttackComp
    {
        private IAttackRelated m_AttackRelated;
        private ISkillComp m_SkillComp;
        private ISkillCaster m_SkillCaster;

        private void Start()
        {
            m_AttackRelated = GetComponent<IAttackRelated>();
            m_SkillComp = GetComponent<ISkillComp>();
            m_SkillCaster = GetComponent<ISkillCaster>();
        }

        public void SuccessAttack(GameObject target)
        {
            if (target.TryGetComponent(out IAttackRelated targetEntity))
            {
                if (targetEntity.GetFaction() == m_AttackRelated.GetFaction())
                    return;

                m_SkillComp.GetSkill(0)?.TakeEffectOn(m_SkillCaster, targetEntity);

                if (target.TryGetComponent(out IHealthComp targetHp))
                    targetHp.TakeDamage(m_AttackRelated);
            }
        }
    }
}