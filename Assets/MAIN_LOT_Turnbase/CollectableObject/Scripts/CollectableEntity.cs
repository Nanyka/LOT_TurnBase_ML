using System.Collections.Generic;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectableEntity : Entity, IAttackRelated, ISkillRelated
    {
        [SerializeField] private CollectableStats[] m_CollectableStats;
        [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private CollectComp m_CollectComp;
        [SerializeField] private AnimateComp m_AnimComp;
        [SerializeField] private SelfAttackComp m_SelfAttackComp;

        private CollectableData m_CollectableData;
        private CollectableStats m_CurrentStat;

        public void Init(CollectableData collectableData)
        {
            m_CollectableData = collectableData;
            RefreshEntity();
        }

        // Remove all listener when entity completed die process
        private void OnDisable()
        {
            OnUnitDie.RemoveAllListeners();
        }

        public override void Relocate(Vector3 position)
        {
            m_Transform.position = position;
        }

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            throw new System.NotImplementedException();
        }

        public override EntityData GetData()
        {
            return m_CollectableData;
        }

        public void DurationDeduct()
        {
            if (m_CurrentStat.IsLongLasting)
                return;

            m_CollectableData.AccumulatedStep++;
            if (m_CollectableData.AccumulatedStep >= m_CurrentStat.MaxTurnToDestroy)
                OnUnitDie.Invoke(this);
        }

        public override FactionType GetFaction()
        {
            return m_CollectableData.FactionType;
        }

        public Vector3 GetPosition()
        {
            throw new System.NotImplementedException();
        }

        public void GainGoldValue() { }

        public bool CheckSelfCollect()
        {
            return m_CurrentStat.IsSelfCollect;
        }

        public CollectableType GetCollectableType()
        {
            return m_CollectableStats[m_CollectableData.CurrentLevel].CollectableType;
        }

        protected virtual void DieIndividualProcess(IAttackRelated killedByEntity)
        {
            // grant effect on killedByEntity
            if (killedByEntity != this && m_CurrentStat._skillEffectType != SkillEffectType.None)
                m_CurrentStat.GetSkillEffect().TakeEffectOn(this, killedByEntity);

            // TODO add animation or effect here
            if (m_AnimComp != null)
                m_AnimComp.SetAnimation(AnimateType.Die);
        }

        #region ATTACK

        public virtual void SuccessAttack(GameObject target)
        {
            throw new System.NotImplementedException();
        }

        public int GetAttackDamage()
        {
            return m_CurrentStat.TrapDamage;
        }

        public void TakeDamage(int damage, IAttackRelated fromEntity)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Skill_SO> GetSkills()
        {
            throw new System.NotImplementedException();
        }

        public EffectComp GetEffectComp()
        {
            throw new System.NotImplementedException();
        }

        public void AccumulateKills()
        {
            throw new System.NotImplementedException();
        }

        public void Attack(Vector3 attackAt)
        {
            m_SelfAttackComp.Attack(attackAt,this, this);
        }

        #endregion
        
        #region SKIN

        public override SkinComp GetSkin()
        {
            return m_SkinComp;
        }

        #endregion
        
        #region SKILL
        
        // public override IEnumerable<Skill_SO> GetSkills()
        // {
        //     throw new System.NotImplementedException();
        // }

        #endregion

        #region EFFECT

        // public override EffectComp GetEffectComp()
        // {
        //     throw new System.NotImplementedException();
        // }

        #endregion

        #region GENERAL

        public virtual void ContributeCommands()
        {
            // If collectable item include currency rewards
            foreach (var command in m_CurrentStat.Commands)
                SavingSystemManager.Instance.StoreCurrencyAtBuildings(command.ToString(), m_CollectableData.Position);

            // If collectable item include creature rewards
            if (m_CurrentStat.SpawnedEntityType == EntityType.NONE)
                return;

            switch (m_CurrentStat.SpawnedEntityType)
            {
                case EntityType.BUILDING:
                    SavingSystemManager.Instance.OnPlaceABuilding(m_CurrentStat.EntityName, m_CollectableData.Position,
                        true);
                    break;
                case EntityType.ENEMY:
                    SavingSystemManager.Instance.OnSpawnMovableEntity(m_CurrentStat.EntityName,
                        m_CollectableData.Position);
                    break;

                case EntityType.RESOURCE:
                    SavingSystemManager.Instance.OnSpawnResource(m_CurrentStat.EntityName, m_CollectableData.Position);
                    break;
            }
        }

        private void RefreshEntity()
        {
            // Set entity stats
            m_CurrentStat = m_CollectableStats[m_CollectableData.CurrentLevel];

            // Initiate entity data if it's new
            m_CollectableData.CollectableType = m_CurrentStat.CollectableType;
            m_CollectableData.SkinAddress = m_CurrentStat.SkinAddress;

            // Retrieve entity data
            if (GameFlowManager.Instance.GameMode == GameMode.ECONOMY ||
                m_CurrentStat.CollectableType != CollectableType.TRAP)
            {
                if (m_AnimComp == null)
                    m_SkinComp.Init(m_CollectableData.SkinAddress);
                else
                    m_SkinComp.Init(m_CollectableData.SkinAddress, m_AnimComp);
            }

            m_CollectComp.Init(OnUnitDie);
            OnUnitDie.AddListener(DieIndividualProcess);
        }

        #endregion
    }
}