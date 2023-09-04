using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class HealthComp : MonoBehaviour
    {
        [SerializeField] private HealthBar _healthBar;

        private int m_MAXHp;
        private int m_MAXStorage;
        private UnityEvent<Entity> _dieEvent;
        private bool _isDeath;

        public void Init(int maxHp, UnityEvent<Entity> dieEvent, EntityData entityData)
        {
            m_MAXHp = maxHp;
            _healthBar.UpdateHealthSlider(entityData.CurrentHp * 1f / m_MAXHp);
            _dieEvent = dieEvent;
            _isDeath = false;

            if (entityData is BuildingData)
            {
                var buildingData = (BuildingData)entityData;
                _healthBar.ShowHealthBar(GameFlowManager.Instance.GameMode == GameMode.ECONOMY,
                    buildingData.BuildingType != BuildingType.TOWER);
                m_MAXStorage = buildingData.StorageCapacity;
            }
            else
                _healthBar.ShowHealthBar(false, false);
        }

        public void TakeDamage(int damage, EntityData entityData, Entity killedBy)
        {
            if (_isDeath)
                return;

            entityData.CurrentHp -= damage;
            _healthBar.UpdateHealthSlider(entityData.CurrentHp * 1f / m_MAXHp);

            if (entityData.CurrentHp <= 0)
                Die(killedBy);
        }

        private void Die(Entity killedByFaction)
        {
            _isDeath = true;
            _dieEvent.Invoke(killedByFaction);
        }

        public void UpdateStorage(int value)
        {
            _healthBar.UpdateStorageSlider(value * 1f / m_MAXStorage);
        }

        public void UpdatePriceText(int price)
        {
            _healthBar.UpdatePrice(price);
        }
    }
}