using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class AttackComp : MonoBehaviour
    {
        public void Attack(IEnumerable<Vector3> attackPoints, Entity mEntity, int jumpStep)
        {
            if (attackPoints == null)
                return;
            
            var mEnvironment = GameFlowManager.Instance.GetEnvManager();
            foreach (var attackPoint in attackPoints)
            {
                var attackFaction = mEnvironment.CheckFaction(attackPoint);
                var target = mEnvironment.GetObjectByPosition(attackPoint, attackFaction);
                if (target == null)
                    continue;

                if (target.TryGetComponent(out Entity targetEntity))
                {
                    var selectedSkill = mEntity.GetSkills().ElementAt(jumpStep - 1);

                    if (selectedSkill.CheckPreAttack() == false)
                    {
                        var skillEffect = selectedSkill.GetSkillEffect();
                        if (skillEffect != null)
                            skillEffect.TakeEffectOn(mEntity, targetEntity);
                    }
                    
                    if (targetEntity.GetFaction() != mEntity.GetFaction())
                        targetEntity.TakeDamage(mEntity.GetAttackDamage(), mEntity);
                }
            }
        }

        public void Attack(Vector3 attackPoint, Entity mEntity, int jumpStep)
        {
            var mEnvironment = GameFlowManager.Instance.GetEnvManager();
            var attackFaction = mEnvironment.CheckFaction(attackPoint);
            var target = mEnvironment.GetObjectByPosition(attackPoint, attackFaction);
            if (target == null)
                return;

            if (target.TryGetComponent(out Entity targetEntity))
            {
                var selectedSkill = mEntity.GetSkills().ElementAt(jumpStep - 1);
                var skillEffect = selectedSkill.GetSkillEffect();
                if (skillEffect != null)
                    skillEffect.TakeEffectOn(mEntity, targetEntity);

                if (targetEntity.GetFaction() != mEntity.GetFaction())
                {
                    // Debug.Log($"Take damage on {targetEntity.name} an amount: {mEntity.GetAttackDamage()}");
                    targetEntity.TakeDamage(mEntity.GetAttackDamage(), mEntity);
                    mEntity.GainGoldValue();
                }
            }
        }
    }
}