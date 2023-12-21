using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTowerAttackComp : MonoBehaviour, IAttackComp
    {
        private IAttackRelated m_AttackRelated;

        private void Start()
        {
            m_AttackRelated = GetComponent<IAttackRelated>();
        }

        public void SuccessAttack(GameObject target)
        {
            if (target.TryGetComponent(out IAttackRelated targetEntity))
            {
                if (targetEntity.GetFaction() == m_AttackRelated.GetFaction())
                    return;

                if (target.TryGetComponent(out IHealthComp targetHp))
                    targetHp.TakeDamage(m_AttackRelated);
            }
        }
    }
}