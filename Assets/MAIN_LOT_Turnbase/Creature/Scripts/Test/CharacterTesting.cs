using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public int _accumulateAttack;
    
}

public interface IAttack
{
    public void Attack(CharacterTesting character);
    
    
}

public class AttackAndDash : IAttack
{
    public void Attack(CharacterTesting character)
    {
        Debug.Log($"{character.transform.position}, add accumulateAttack: {character.m_Data._accumulateAttack++}");
        Debug.Log("Dash");
    }

    private IEnumerator Dash()
    {
        yield return new WaitForSeconds(1f);
    }
}

public class OnlyDash : IAttack
{
    public void Attack(CharacterTesting character)
    {
        Debug.Log($"{character.transform.position}");
    }
}

public enum AttackPatternType
{
    NONE,
    ATTACKANDDASH
}

public class AttackPattern : ScriptableObject
{
    public AttackPatternType m_AttackType;
    public IAttack m_AttackPattern;

    public void ExecuteAttack(CharacterTesting characterTesting)
    {
        if (m_AttackPattern == null)
            InitiateAttackPattern();
        
        m_AttackPattern.Attack(characterTesting);
    }

    private void InitiateAttackPattern()
    {
        switch (m_AttackType)
        {
            case AttackPatternType.ATTACKANDDASH:
                m_AttackPattern = new AttackAndDash();
                break;
        }
    }
}

public class CharacterTesting : MonoBehaviour
{
    public CharacterData m_Data { get; private set; }
    [SerializeField] private AttackPattern m_AttackPattern;
    
    public void KindOfAction()
    {
        m_AttackPattern.ExecuteAttack(this);
    }
    
    
}
