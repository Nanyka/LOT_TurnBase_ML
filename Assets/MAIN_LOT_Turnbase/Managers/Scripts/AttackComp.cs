using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AttackComp : MonoBehaviour
    {
        public void Attack(IEnumerable<Vector3> attackPoints, Entity mEntity, int damage,
            EnvironmentManager mEnvironment)
        {
            if (attackPoints == null)
                return;

            foreach (var attackPoint in attackPoints)
            {
                var attackFaction = mEnvironment.CheckFaction(attackPoint);
                if (attackFaction != mEntity.GetFaction())
                {
                    var enemy = mEnvironment.GetObjectByPosition(attackPoint, attackFaction);
                    if (enemy == null)
                        continue;

                    if (enemy.TryGetComponent(out Entity enemyEntity))
                    {
                        enemyEntity.TakeDamage(damage, mEntity);
                    }
                }
            }
        }
    }
}