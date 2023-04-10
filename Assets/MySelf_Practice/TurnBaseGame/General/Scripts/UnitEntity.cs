using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEntity : MonoBehaviour
{
    [SerializeField] private UnitStats m_UnitStats;
    [SerializeField] private HealthComp m_HealthComp;
    [SerializeField] private UnitSkill m_UnitSkill;

    private void Start()
    {
        m_HealthComp.Init(m_UnitStats.HealthPoint);
    }

    #region HEALTH

    public void TakeDamage(int damage)
    {
        m_HealthComp.TakeDamage(damage);
    }

    #endregion

    #region GET
    public IEnumerable<Vector3> GetAttackPoint(Vector3 midPos, Vector3 direction, int jumpStep)
    {
        return m_UnitSkill.AttackPoints(midPos, direction, jumpStep);
    }

    public int GetAttackDamage()
    {
        return m_UnitStats.Strengh;
    }

    #endregion
}
