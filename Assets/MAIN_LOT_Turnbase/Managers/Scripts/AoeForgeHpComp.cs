using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class AoeForgeHpComp : MonoBehaviour, IHealthComp, IRemoveEntity
    {
        private IEntityUIUpdate entityUI;
        private UnityEvent<IAttackRelated> _dieEvent;
        private int m_MAXHp;
        private EntityData m_Data;
        private bool _isDeath;

        public void Init(int maxHp, UnityEvent<IAttackRelated> dieEvent, EntityData entityData)
        {
            entityUI = GetComponent<IEntityUIUpdate>();
            m_Data = entityData;
            m_MAXHp = maxHp;
            _dieEvent = dieEvent;
            
            entityUI.UpdateHealthSlider(entityData.CurrentHp * 1f / m_MAXHp);
            entityUI.ShowBars(false, true, true);
            _isDeath = false;
        }

        public void TakeDamage(IAttackRelated attackBy)
        {
            if (_isDeath)
                return;

            m_Data.CurrentHp -= attackBy.GetAttackDamage();
            entityUI.UpdateHealthSlider(m_Data.CurrentHp * 1f / m_MAXHp);

            if (m_Data.CurrentHp <= 0)
            {
                attackBy.AccumulateKills();
                Die(attackBy);
            }
        }

        public void Die(IAttackRelated killedByFaction)
        {
            _isDeath = true;
            _dieEvent.Invoke(killedByFaction);

            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);
            StartCoroutine(DestroyVisual());
        }

        public bool CheckAlive()
        {
            return !_isDeath;
        }

        private IEnumerator DestroyVisual()
        {
            // Add VFX
            yield return new WaitForSeconds(1f);
            Debug.Log("Remove building");
            gameObject.SetActive(false);
        }

        public GameObject GetRemovedObject()
        {
            return gameObject;
        }

        public EntityData GetEntityData()
        {
            return m_Data;
        }
    }
}