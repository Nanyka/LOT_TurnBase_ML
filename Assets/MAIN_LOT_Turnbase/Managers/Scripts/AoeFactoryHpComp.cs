using System;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class AoeFactoryHpComp : MonoBehaviour, IHealthComp
    {
        private IEntityUIUpdate entityUI;
        private int m_MAXHp;
        // private int m_MAXStorage; // TODO: move it to FactoryComp
        private bool _isDeath;

        public void Init(int maxHp, UnityEvent<IAttackRelated> dieEvent, EntityData entityData)
        {
            entityUI = GetComponent<IEntityUIUpdate>();
            m_MAXHp = maxHp;
            entityUI.UpdateHealthSlider(entityData.CurrentHp * 1f / m_MAXHp);
            entityUI.ShowBars(false,true,true);
            _isDeath = false;

            // var buildingData = (BuildingData)entityData;
            // m_MAXStorage = buildingData.StorageCapacity;
        }

        public void TakeDamage(IAttackRelated attackBy)
        {
            throw new System.NotImplementedException();
        }

        public void Die(IAttackRelated killedByFaction)
        {
            throw new NotImplementedException();
        }
    }
}