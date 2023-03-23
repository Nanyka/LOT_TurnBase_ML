using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkill : MonoBehaviour
{
    [SerializeField] private Skill_SO[] m_SkillSOs;

    public IEnumerable<Vector3> AttackPoints(Vector3 targetPos, Vector3 direction, int jumpStep)
    {
        return m_SkillSOs[jumpStep-1].CalculateSkillRange(targetPos,direction);
    }

    public float GetSkillMagnitude(int jumpStep)
    {
        return m_SkillSOs[jumpStep].GetMagnitude();
    }
}
