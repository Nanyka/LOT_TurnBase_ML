using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class AttackComp : MonoBehaviour
    {
        public void Attack(IEnumerable<Vector3> attackPoints, IAttackRelated mEntity, ISkillRelated mSkill,
            int jumpStep)
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

                if (target.TryGetComponent(out IAttackRelated targetEntity))
                {
                    var selectedSkill = mEntity.GetSkills().ElementAt(jumpStep - 1);

                    if (selectedSkill.CheckPreAttack() == false)
                    {
                        var skillEffect = selectedSkill.GetSkillEffect();
                        if (skillEffect != null)
                            skillEffect.TakeEffectOn(mSkill, targetEntity);
                    }

                    if (targetEntity.GetFaction() != mEntity.GetFaction())
                    {
                        if (target.TryGetComponent(out IHealthComp healthComp))
                            healthComp.TakeDamage(mEntity);
                    }

                    // targetEntity.TakeDamage(mEntity.GetAttackDamage(), mEntity);
                }
            }
        }

        public void Attack(Vector3 attackPoint, IAttackRelated mEntity, ISkillRelated mSkill, int jumpStep)
        {
            var mEnvironment = GameFlowManager.Instance.GetEnvManager();
            var attackFaction = mEnvironment.CheckFaction(attackPoint);
            var target = mEnvironment.GetObjectByPosition(attackPoint, attackFaction);
            if (target == null)
                return;

            if (target.TryGetComponent(out IAttackRelated attackRelated))
            {
                var selectedSkill = mEntity.GetSkills().ElementAt(jumpStep - 1);
                var skillEffect = selectedSkill.GetSkillEffect();
                if (skillEffect != null)
                    skillEffect.TakeEffectOn(mSkill, attackRelated);

                if (attackRelated.GetFaction() != mEntity.GetFaction())
                {
                    if (target.TryGetComponent(out IHealthComp healthComp))
                        healthComp.TakeDamage(mEntity);
                    mEntity.GainGoldValue(); // segregate it into ICollectGold interface
                }
            }

            // if (target.TryGetComponent(out Entity targetEntity))
            // {
            //     var selectedSkill = mEntity.GetSkills().ElementAt(jumpStep - 1);
            //     var skillEffect = selectedSkill.GetSkillEffect();
            //     if (skillEffect != null)
            //         skillEffect.TakeEffectOn(mEntity, targetEntity);
            //
            //     if (targetEntity.GetFaction() != mEntity.GetFaction())
            //     {
            //         targetEntity.TakeDamage(mEntity.GetAttackDamage(), mEntity);
            //         mEntity.GainGoldValue();
            //     }
            // }
        }
    }
}