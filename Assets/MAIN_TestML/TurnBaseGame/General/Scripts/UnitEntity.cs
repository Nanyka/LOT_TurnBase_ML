using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitEntity : MonoBehaviour
{
    [NonSerialized] public UnityEvent OnUnitDie = new(); 
    
    [Header("Default components")] 
    [SerializeField] private Animator m_Animator;
    
    [Header("Custom components")]
    [SerializeField] private UnitStats m_UnitStats;
    [SerializeField] private UnitSkill m_UnitSkill;
    [SerializeField] private HealthComp m_HealthComp;
    [SerializeField] private AttackComp m_AttackComp;
    [SerializeField] private EffectComp m_EffectComp;
    [SerializeField] private AttackPath m_AttackPath;

    [SerializeField] private UnitData data;
    private IGetUnitInfo m_Info;
    private (Vector3 midPos, Vector3 direction, int jumpStep, int faction) _currentState;

    private void Start()
    {
        m_HealthComp.Init(m_UnitStats.HealthPoint, OnUnitDie, ref data);
    }

    #region HEALTH

    public void TakeDamage(int damage)
    {
        m_HealthComp.TakeDamage(damage, ref data);
    }

    public int GetCurrentHealth()
    {
        return m_HealthComp.GetCurrentHealth(ref data);
    }

    #endregion

    #region ATTACK

    public void Attack(IGetUnitInfo unitInfo)
    {
        m_Info = unitInfo;
        _currentState = unitInfo.GetCurrentState();
        
        m_Animator.SetTrigger(m_UnitSkill.GetAttackAnimation(_currentState.jumpStep-1));
    }
    
    // Use ANIMATION's EVENT to take damage enemy and keep effect be execute simultaneously
    public void AttackInAnim()
    {
        var attackRange =
            m_UnitSkill.AttackPoints(_currentState.midPos, _currentState.direction, _currentState.jumpStep);
        var attackPoints = attackRange as Vector3[] ?? attackRange.ToArray();
        m_AttackComp.Attack(attackPoints, _currentState.faction, m_Info.GetUnitInfo().damage, m_Info.GetEnvironment());
        ShowAttackRange(attackPoints);
        
        m_EffectComp.AttackVFX(_currentState.jumpStep);
    }

    public void ShowAttackRange(IEnumerable<Vector3> attackRange)
    {
        if (m_AttackPath is not null) m_AttackPath.AttackAt(attackRange);
    }
    
    public IEnumerable<Vector3> GetAttackPoint(Vector3 midPos, Vector3 direction, int jumpStep)
    {
        return m_UnitSkill.AttackPoints(midPos, direction, jumpStep);
    }

    public int GetAttackDamage()
    {
        return m_UnitStats.Strengh;
    }

    #endregion

    #region SKILL
    
    public IEnumerable<Skill_SO> GetSkills()
    {
        return m_UnitSkill.GetSkills();
    }

    #endregion

    #region GENERAL
    
    public void SetWalkAnimation(bool isWalk)
    {
        m_Animator.SetBool("Walk", isWalk);
    }
    
    public void ResetEntity()
    {
        m_HealthComp.Reset(ref data);
    }

    #endregion
}