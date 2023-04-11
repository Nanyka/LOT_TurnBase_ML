using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthComp : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;

    private int m_MAXHp;
    private int _currentHp;
    private UnityEvent _dieEvent;

    public void Init(int maxHp, UnityEvent dieEvent)
    {
        m_MAXHp = maxHp;
        _currentHp = m_MAXHp;
        _hpSlider.value = _currentHp * 1f / m_MAXHp;
        _dieEvent = dieEvent;
    }

    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        _hpSlider.value = _currentHp * 1f / m_MAXHp;

        if (_currentHp <= 0)
            Die();
    }

    public int GetCurrentHealth()
    {
        return _currentHp;
    }

    private void Die()
    {
        _dieEvent.Invoke();
    }

    public void Reset()
    {
        _currentHp = m_MAXHp;
        _hpSlider.value = _currentHp * 1f / m_MAXHp;
    }
}
