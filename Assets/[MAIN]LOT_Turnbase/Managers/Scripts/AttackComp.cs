using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AttackComp : MonoBehaviour
    {
        public void Attack(IEnumerable<Vector3> attackPoints, FactionType m_Faction, int damage, EnvironmentManager m_Environment)
        {
            if (attackPoints == null)
                return;

            foreach (var attackPoint in attackPoints)
            {
                if (m_Environment.CheckEnemy(attackPoint, m_Faction))
                {
                    var enemy = m_Environment.GetEnemyByPosition(attackPoint, m_Faction);
                    if (enemy.TryGetComponent(out Entity enemyEntity))
                        enemyEntity.TakeDamage(damage);
                }
            }
        }
    }
}