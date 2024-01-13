using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class AoeTutorialHpComp : MonoBehaviour, IHealthComp
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
            m_Data.CurrentHp = maxHp;
            _dieEvent = dieEvent;

            m_Collider.enabled = true;
            entityUI.UpdateHealthSlider(m_Data.CurrentHp * 1f / m_MAXHp);
            entityUI.ShowBars(false,true,false);
        }

        public void TakeDamage(IAttackRelated attackBy)
        {
            if (_isDeath)
                return;

            Debug.Log($"Attack by: {attackBy.GetAttackDamage()}");
            m_Data.CurrentHp -= attackBy.GetAttackDamage();
            entityUI.UpdateHealthSlider(m_Data.CurrentHp * 1f / m_MAXHp);

            // if (m_Data.CurrentHp <= 0)
            // {
            //     m_AnimateComp.SetAnimation(AnimateType.Die);
            // }
            // else
            //     m_AnimateComp.SetAnimation(AnimateType.TakeDamage);
        }

        public void HideTheEntity()
        {
            throw new System.NotImplementedException();
        }

        public bool CheckAlive()
        {
            throw new System.NotImplementedException();
        }
    }
}