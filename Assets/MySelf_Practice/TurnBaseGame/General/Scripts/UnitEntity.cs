using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitEntity : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnUnitDie = new(); 
    
    [Header("Default components")] 
    [SerializeField] private Animator m_Animator;
    
    [Header("Custom components")]
    [SerializeField] private UnitStats m_UnitStats;
    [SerializeField] private HealthComp m_HealthComp;
    [SerializeField] private UnitSkill m_UnitSkill;
    [SerializeField] private AttackComp m_AttackComp;

    private void Start()
    {
        m_HealthComp.Init(m_UnitStats.HealthPoint, OnUnitDie);
    }

    #region HEALTH

    public void TakeDamage(int damage)
    {
        m_HealthComp.TakeDamage(damage);
    }

    public int GetCurrentHealth()
    {
        return m_HealthComp.GetCurrentHealth();
    }

    #endregion

    #region ATTACK

    public void Attack(IGetUnitInfo unitInfo)
    {
        var info = unitInfo.GetUnitInfo();
        var currentState = unitInfo.GetCurrentState();

        m_AttackComp.Attack(
            m_UnitSkill.AttackPoints(currentState.midPos, currentState.direction, currentState.jumpStep),
            currentState.faction, info.damage, unitInfo.GetEnvironment());
        
        m_Animator.SetTrigger(m_UnitSkill.GetAttackAnimation(currentState.jumpStep-1));
    }

    #endregion

    #region GET & SET

    public IEnumerable<Vector3> GetAttackPoint(Vector3 midPos, Vector3 direction, int jumpStep)
    {
        return m_UnitSkill.AttackPoints(midPos, direction, jumpStep);
    }

    public int GetAttackDamage()
    {
        return m_UnitStats.Strengh;
    }

    public void ResetEntity()
    {
        m_HealthComp.Reset();
    }
    
    public IEnumerable<Skill_SO> GetSkills()
    {
        return m_UnitSkill.GetSkills();
    }

    public void SeWalkAnimation(bool isWalk)
    {
        m_Animator.SetBool("Walk", isWalk);
    }

    #endregion
}