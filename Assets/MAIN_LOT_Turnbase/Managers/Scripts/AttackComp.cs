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
                    if (targetEntity.GetFaction() != mEntity.GetFaction())
                        targetEntity.TakeDamage(mEntity.GetAttackDamage(), mEntity);
                    
                    var skillEffect = mEntity.GetSkills().ElementAt(jumpStep-1).GetSkillEffect();
                    if (skillEffect != null)
                        skillEffect.TakeEffectOn(mEntity, targetEntity);
                }
            }

            // AttackVFX(jumpStep);
        }

        // private void AttackVFX(int vfxIndex)
        // {
        //     _attackVFX[vfxIndex - 1]?.Play();
        // }
    }
}