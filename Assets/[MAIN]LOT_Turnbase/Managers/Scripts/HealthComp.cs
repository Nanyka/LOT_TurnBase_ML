using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LOT_Turnbase
{
    public class HealthComp : MonoBehaviour
    {
        [SerializeField] private Slider _hpSlider;

        private int m_MAXHp;
        private UnityEvent _dieEvent;

        public void Init(int maxHp, UnityEvent dieEvent, ref UnitData unitData)
        {
            m_MAXHp = maxHp;
            unitData.CurrentHealth = m_MAXHp;
            _hpSlider.value = unitData.CurrentHealth * 1f / m_MAXHp;
            _dieEvent = dieEvent;
        }

        public void TakeDamage(int damage, ref UnitData unitData)
        {
            unitData.CurrentHealth -= damage;
            _hpSlider.value = unitData.CurrentHealth * 1f / m_MAXHp;

            if (unitData.CurrentHealth <= 0)
                Die();
        }

        public int GetCurrentHealth(ref UnitData unitData)
        {
            return unitData.CurrentHealth;
        }

        private void Die()
        {
            _dieEvent.Invoke();
        }

        public void Reset(ref UnitData unitData)
        {
            unitData.CurrentHealth = m_MAXHp;
            _hpSlider.value = unitData.CurrentHealth * 1f / m_MAXHp;
        }
    }
}