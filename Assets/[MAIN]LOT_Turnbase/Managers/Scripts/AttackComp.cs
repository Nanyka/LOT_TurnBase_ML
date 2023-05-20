using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class AttackComp : MonoBehaviour
    {
        public void Attack(IEnumerable<Vector3> attackPoints, int m_Faction, int damage, ICheckEnemyPosition m_Environment)
        {
            if (attackPoints == null)
                return;

            foreach (var attackPoint in attackPoints)
            {
                if (m_Environment.CheckEnemy(attackPoint, m_Faction))
                {
                    var enemy = m_Environment.GetEnemyByPosition(attackPoint, m_Faction);
                    if (enemy.TryGetComponent(out UnitEntity enemyEntity))
                        enemyEntity.TakeDamage(damage);
                }
            }
        }
    }
}