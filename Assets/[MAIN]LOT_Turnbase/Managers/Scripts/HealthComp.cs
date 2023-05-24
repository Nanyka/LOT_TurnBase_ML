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

        public void TakeDamage(int damage, EntityData entityData)
        {
            entityData.CurrentHp -= damage;
            _hpSlider.value = entityData.CurrentHp * 1f / m_MAXHp;

            if (entityData.CurrentHp <= 0)
                Die();
        }

        public int GetCurrentHealth(EntityData entityData)
        {
            return entityData.CurrentHp;
        }

        private void Die()
        {
            _dieEvent.Invoke();
        }

        public void Reset(EntityData entityData)
        {
            entityData.CurrentHp = m_MAXHp;
            _hpSlider.value = entityData.CurrentHp * 1f / m_MAXHp;
        }
    }
}