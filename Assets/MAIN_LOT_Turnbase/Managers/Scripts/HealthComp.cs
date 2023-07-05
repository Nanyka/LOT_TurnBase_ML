using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class HealthComp : MonoBehaviour
    {
        [SerializeField] private Slider _hpSlider;

        private int m_MAXHp;
        private UnityEvent<Entity> _dieEvent;
        private bool _isDeath;

        public void Init(int maxHp, UnityEvent<Entity> dieEvent, EntityData entityData)
        {
            m_MAXHp = maxHp;
            _hpSlider.value = entityData.CurrentHp * 1f / m_MAXHp;
            _dieEvent = dieEvent;
            _isDeath = false;
        }

        public void TakeDamage(int damage, EntityData entityData, Entity killedBy)
        {
            if (_isDeath)
                return;

            entityData.CurrentHp -= damage;
            _hpSlider.value = entityData.CurrentHp * 1f / m_MAXHp;

            if (entityData.CurrentHp <= 0)
                Die(killedBy);
        }

        private void Die(Entity killedByFaction)
        {
            _isDeath = true;
            _dieEvent.Invoke(killedByFaction);
        }
    }
}