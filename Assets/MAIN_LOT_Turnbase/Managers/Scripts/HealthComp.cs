using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class HealthComp : MonoBehaviour
    {
        [NonSerialized] public UnityEvent<float> TakeDamageEvent = new(); 

        [SerializeField] private EntityUI entityUI;

        private int m_MAXHp;
        private int m_MAXStorage;
        private UnityEvent<Entity> _dieEvent;
        private bool _isDeath;

        public void Init(int maxHp, UnityEvent<Entity> dieEvent, EntityData entityData)
        {
            m_MAXHp = maxHp;
            entityUI.UpdateHealthSlider(entityData.CurrentHp * 1f / m_MAXHp);
            _dieEvent = dieEvent;
            _isDeath = false;

            if (entityData is BuildingData)
            {
                var buildingData = (BuildingData)entityData;
                entityUI.ShowBars(GameFlowManager.Instance.GameMode == GameMode.ECONOMY,
                    buildingData.BuildingType != BuildingType.MAINHALL || GameFlowManager.Instance.GameMode != GameMode.ECONOMY,
                    buildingData.BuildingType != BuildingType.TOWER);
                m_MAXStorage = buildingData.StorageCapacity;

                if (GameFlowManager.Instance.GameMode == GameMode.BATTLE)
                {
                    entityUI.UpdatePrice(buildingData.StorageCurrency.ToString(), buildingData.CurrentStorage);
                }
            }
            else if (entityData is CreatureData)
            {
                var creatureData = (CreatureData)entityData;
                entityUI.ShowBars(GameFlowManager.Instance.GameMode != GameMode.ECONOMY, true, false);
                entityUI.UpdatePrice(creatureData.CurrentExp);
            }
            else
                entityUI.ShowBars(false, true, false);
            
            TakeDamageEvent.Invoke(entityData.CurrentHp * 1f / m_MAXHp);
        }

        public void TakeDamage(int damage, EntityData entityData, Entity killedBy)
        {
            if (_isDeath)
                return;

            entityData.CurrentHp -= damage;
            var healthPortion = entityData.CurrentHp * 1f / m_MAXHp;
            entityUI.UpdateHealthSlider(healthPortion);
            TakeDamageEvent.Invoke(healthPortion);

            if (entityData.CurrentHp <= 0)
            {
                if (killedBy.TryGetComponent(out CreatureEntity creatureEntity))
                    creatureEntity.AccumulateKills();

                Die(killedBy);
            }
        }

        private void Die(Entity killedByFaction)
        {
            _isDeath = true;
            _dieEvent.Invoke(killedByFaction);
        }

        public void UpdateStorage(int value)
        {
            entityUI.UpdateStorageSlider(value * 1f / m_MAXStorage);
        }

        public void UpdatePriceText(int price)
        {
            entityUI.UpdatePrice(price);
        }

        public void TurnHealthSlider(bool isOn)
        {
            entityUI.TurnHealthSlider(isOn);
        }
    }
}