using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class AoeTroopHpComp : MonoBehaviour, IHealthComp, IRemoveEntity
    {
        private Collider m_Collider;
        private IEntityUIUpdate entityUI;
        private IAnimateComp m_AnimateComp;
        private UnityEvent<IAttackRelated> _dieEvent;
        private int m_MAXHp;
        private EntityData m_Data;
        private bool _isDeath;

        public void Init(int maxHp, UnityEvent<IAttackRelated> dieEvent, EntityData entityData)
        {
            m_Collider = GetComponent<Collider>();
            entityUI = GetComponent<IEntityUIUpdate>();
            m_AnimateComp = GetComponent<IAnimateComp>();
            m_MAXHp = maxHp;
            m_Data = entityData;
            _dieEvent = dieEvent;

            m_Collider.enabled = true;
            entityUI.UpdateHealthSlider(m_Data.CurrentHp * 1f / m_MAXHp);
            entityUI.ShowBars(false,true,false);
            _isDeath = false;
        }

        public void TakeDamage(IAttackRelated attackBy)
        {
            if (_isDeath)
                return;

            // TODO: How to retrieve attack damage from attackBy entity
            m_Data.CurrentHp -= attackBy.GetAttackDamage();
            entityUI.UpdateHealthSlider(m_Data.CurrentHp * 1f / m_MAXHp);

            if (m_Data.CurrentHp <= 0)
            {
                attackBy.AccumulateKills();
                m_AnimateComp.SetAnimation(AnimateType.Die);
                Die(attackBy);
            }
            else
                m_AnimateComp.SetAnimation(AnimateType.TakeDamage);
        }

        protected virtual void Die(IAttackRelated killedByFaction)
        {
            _isDeath = true;
            m_Collider.enabled = false;
            _dieEvent.Invoke(killedByFaction);

            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);
            StartCoroutine(DestroyVisual());
        }

        public void HideTheEntity()
        {
            Die(null);
        }

        public bool CheckAlive()
        {
            return !_isDeath;
        }

        private IEnumerator DestroyVisual()
        {
            // Add VFX
            yield return new WaitForSeconds(2f);
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
    
    public interface IHealthComp
    {
        public void Init(int maxHp, UnityEvent<IAttackRelated> dieEvent, EntityData entityData);
        public void TakeDamage(IAttackRelated attackBy);
        public void HideTheEntity(); // Mostly use for transform period when the old entity must be remove for replacing with a new one
        public bool CheckAlive();
    }
}