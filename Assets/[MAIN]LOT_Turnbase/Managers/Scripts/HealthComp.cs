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
        
        public void Init(int maxHp, UnityEvent dieEvent, EntityData entityData)
        {
            m_MAXHp = maxHp;
            _hpSlider.value = entityData.CurrentHp * 1f / m_MAXHp;
            _dieEvent = dieEvent;
        }

        public void TakeDamage(int damage, EntityData entityData)
        {
            entityData.CurrentHp -= damage;
            _hpSlider.value = entityData.CurrentHp * 1f / m_MAXHp;

            if (entityData.CurrentHp <= 0)
                Die();
        }

        private void Die()
        {
            _dieEvent.Invoke();
        }
    }
}