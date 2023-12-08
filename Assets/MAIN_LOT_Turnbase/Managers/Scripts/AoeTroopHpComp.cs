using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class AoeTroopHpComp : MonoBehaviour, IHealthComp
    {
        private IEntityUIUpdate entityUI;
        private int m_MAXHp;
        private bool _isDeath;

        public void Init(int maxHp, UnityEvent<IAttackRelated> dieEvent, EntityData entityData)
        {
            entityUI = GetComponent<IEntityUIUpdate>();
            m_MAXHp = maxHp;
            entityUI.UpdateHealthSlider(entityData.CurrentHp * 1f / m_MAXHp);
            entityUI.ShowBars(false,true,false);
            _isDeath = false;
        }

        public void TakeDamage(int damage, EntityData entityData, IAttackRelated killedBy)
        {
            throw new System.NotImplementedException();
        }
    }
}