using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class ResourceEntity : Entity
    {
        [SerializeField] private ResourceStats m_ResourceStats;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private AnimateComp m_AnimateComp;

        private ResourceData m_ResourceData;

        public void Init(ResourceData resourceData)
        {
            m_ResourceData = resourceData;
            RefreshEntity();
        }

        #region RESOURCE DATA

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            m_Transform.position = position;
        }

        public void DurationDeduct()
        {
            if (m_ResourceStats.IsLongLasting)
                return;

            m_ResourceData.AccumulatedStep++;
            if (m_ResourceData.AccumulatedStep >= m_ResourceStats.MaxTurnToDestroy)
                OnUnitDie.Invoke(this);
        }

        public ResourceData GetResourceData()
        {
            return m_ResourceData;
        }

        public override CommandName GetCommand()
        {
            return m_ResourceStats.Command;
        }

        public override FactionType GetFaction()
        {
            throw new System.NotImplementedException();
        }

        public override void CollectExp(int expAmount)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region HEALTH DATA

        public override void TakeDamage(int damage, Entity fromEntity)
        {
            m_HealthComp.TakeDamage(damage, m_ResourceData, fromEntity);
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public override int GetCurrentHealth()
        {
            throw new System.NotImplementedException();
        }

        public override void DieCollect(Entity killedByEntity)
        {
            // TODO add animation or effect here
        }

        #endregion

        #region ATTACK

        public override void AttackSetup(IGetCreatureInfo unitInfo)
        {
            throw new System.NotImplementedException();
        }

        public override int GetAttackDamage()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region SKILL

        public override IEnumerable<Skill_SO> GetSkills()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region ANIMATION

        public override void SetAnimation(AnimateType animation, bool isTurnOn)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public override void RefreshEntity()
        {
            m_Transform.position = m_ResourceData.Position;
            m_Transform.eulerAngles = m_ResourceData.Rotation;
            m_HealthComp.Init(m_ResourceStats.MaxHp, OnUnitDie, m_ResourceData);
            OnUnitDie.AddListener(DieCollect);
        }
    }
}