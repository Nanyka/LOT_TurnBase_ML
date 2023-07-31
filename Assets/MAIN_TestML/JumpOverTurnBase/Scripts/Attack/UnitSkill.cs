using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkill : MonoBehaviour
{
    [SerializeField] private Skill_SO[] m_SkillSOs;

    public IEnumerable<Vector3> AttackPoints(Vector3 targetPos, Vector3 direction, int jumpStep)
    {
        if (m_SkillSOs.Length < jumpStep || m_SkillSOs[jumpStep - 1] == null)
            return null;
        return m_SkillSOs[jumpStep - 1].CalculateSkillRange(targetPos, direction);
    }

    public float GetSkillMagnitude(int jumpStep)
    {
        return m_SkillSOs[jumpStep-1].GetDuration();
    }

    public IEnumerable<Skill_SO> GetSkills()
    {
        return m_SkillSOs;
    }

    public Skill_SO GetSkillByIndex(int index)
    {
        return m_SkillSOs[index];
    }

    public int GetSkillAmount()
    {
        return m_SkillSOs.Length;
    }

    public string GetAttackAnimation(int atIndex)
    {
        return m_SkillSOs[atIndex].GetAnimation();
    }
}