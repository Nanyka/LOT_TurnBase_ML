using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class AoeTowerHpComp : MonoBehaviour, IHealthComp, IRemoveEntity
    {
        private IEntityUIUpdate entityUI;

        private IGetEntityData<BuildingStats> m_Data;

        // private UnityEvent<IAttackRelated> _dieEvent;
        private int m_MAXHp;
        private bool _isDeath;

        public void Init(int maxHp, UnityEvent<IAttackRelated> dieEvent, EntityData entityData)
        {
            entityUI = GetComponent<IEntityUIUpdate>();
            m_Data = GetComponent<IGetEntityData<BuildingStats>>();
            m_MAXHp = maxHp;
            entityUI.UpdateHealthSlider(entityData.CurrentHp * 1f / m_MAXHp);
            entityUI.ShowBars(false, true, true);
            // _dieEvent = dieEvent;
            _isDeath = false;
        }

        public void TakeDamage(EntityData mEntityData, IAttackRelated killedBy)
        {
            if (_isDeath)
                return;

            mEntityData.CurrentHp -= killedBy.GetAttackDamage();
            var healthPortion = mEntityData.CurrentHp * 1f / m_MAXHp;
            entityUI.UpdateHealthSlider(healthPortion);

            if (mEntityData.CurrentHp <= 0)
            {
                killedBy.AccumulateKills();
                Die(killedBy);
            }
        }

        private void Die(IAttackRelated killedByFaction)
        {
            _isDeath = true;
            // _dieEvent.Invoke(killedByFaction);

            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);
            StartCoroutine(DestroyVisual());
        }

        private IEnumerator DestroyVisual()
        {
            // Add VFX
            yield return new WaitForSeconds(1f);
            Debug.Log("Remove building");
            gameObject.SetActive(false);
        }

        public void Remove(IEnvironmentLoader envLoader)
        {
            envLoader.GetData().BuildingData.Remove((BuildingData)m_Data.GetData());
        }

        public GameObject GetRemovedObject()
        {
            return gameObject;
        }

        public EntityData GetEntityData()
        {
            return m_Data.GetData();
        }
    }
}