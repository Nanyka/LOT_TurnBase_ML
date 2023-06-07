using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class HealthComp : MonoBehaviour
    {
        [SerializeField] private Slider _hpSlider;

        private int m_MAXHp;
        private UnityEvent<FactionType> _dieEvent;
        
        public void Init(int maxHp, UnityEvent<FactionType> dieEvent, EntityData entityData)
        {
            m_MAXHp = maxHp;
            _hpSlider.value = entityData.CurrentHp * 1f / m_MAXHp;
            _dieEvent = dieEvent;
        }

        public void TakeDamage(int damage, EntityData entityData, FactionType killedByFaction)
        {
            entityData.CurrentHp -= damage;
            _hpSlider.value = entityData.CurrentHp * 1f / m_MAXHp;

            if (entityData.CurrentHp <= 0)
                Die(killedByFaction);
        }

        private void Die(FactionType killedByFaction)
        {
            _dieEvent.Invoke(killedByFaction);
        }
    }
}