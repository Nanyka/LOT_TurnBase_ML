using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthComp : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;

    private int m_MAXHp;
    private int _currentHp;

    public void Init(int maxHp)
    {
        m_MAXHp = maxHp;
        _currentHp = m_MAXHp;
        _hpSlider.value = _currentHp * 1f / m_MAXHp;
    }

    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        _hpSlider.value = _currentHp * 1f / m_MAXHp;
        Debug.Log($"{name} take {damage} damge and remain health is {_hpSlider.value*100f}");

        if (_currentHp <= 0)
            Die();
    }

    private void Die()
    {
        // TODO Die function
        Debug.Log($"{name} die");
    }
}
