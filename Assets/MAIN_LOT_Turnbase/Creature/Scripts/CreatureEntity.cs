using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class CreatureEntity : Entity, IStatsProvider<UnitStats>
    {
        [SerializeField] private UnitStats[] m_UnitStats;
        [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
        [SerializeField] private SkillComp m_SkillComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private AttackPath m_AttackPath;
        [SerializeField] private AnimateComp m_AnimateComp;

        [SerializeField] private CreatureData m_CreatureData;
        private UnitStats m_CurrentStat;
        private IGetCreatureInfo m_Info;
        private bool _isDie;

        public void Init(CreatureData creatureData)
        {
            m_CreatureData = creatureData;
            RefreshEntity();
        }

        // Remove all listener when entity completed die process
        private void OnDisable()
        {
            OnUnitDie.RemoveAllListeners();
        }

        #region CREATURE DATA

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            m_Transform.position = position;
            m_Transform.eulerAngles = rotation;

            m_CreatureData.Position = position;
            m_CreatureData.Rotation = rotation;
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public override EntityData GetData()
        {
            return m_CreatureData;
        }

        public override FactionType GetFaction()
        {
            return m_CreatureData.FactionType;
        }

        public override int GetExpReward()
        {
            return m_CurrentStat.ExpReward;
        }

        public override void CollectExp(int expAmount)
        {
            m_CreatureData.CurrentExp += expAmount;
            if (m_CreatureData.CurrentExp >= m_CurrentStat.ExpToLevelUp &&
                m_CreatureData.CurrentLevel + 1 < m_UnitStats.Length)
            {
                // Level up
                m_CreatureData.CurrentLevel++;

                // Reset stats and appearance
                m_CurrentStat = m_UnitStats[m_CreatureData.CurrentLevel];
                var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_CreatureData.EntityName);
                m_CreatureData.SkinAddress = inventoryItem.skinAddress[m_CreatureData.CurrentLevel];
                m_CreatureData.CurrentDamage = m_CurrentStat.Strengh;
                m_SkinComp.Init(m_CreatureData.SkinAddress, m_AnimateComp);

                // SavingSystemManager.Instance.OnCheckExpandMap.Invoke();
            }
        }

        #endregion

        #region SKIN

        public void SetActiveMaterial()
        {
            m_SkinComp.SetActiveMaterial();
        }

        public void SetDisableMaterial()
        {
            m_SkinComp.SetDisableMaterial();
        }

        #endregion

        #region HEALTH

        public override void TakeDamage(int damage, Entity fromEntity)
        {
            m_HealthComp.TakeDamage(damage, m_CreatureData, fromEntity);
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public override int GetCurrentHealth()
        {
            return m_CreatureData.CurrentHp;
        }

        public override void DieIndividualProcess(Entity killedByEntity)
        {
            // Set animation and effect when entity die here
            m_AnimateComp.SetAnimation(AnimateType.Die);
        }

        #endregion

        #region ATTACK

        public override void AttackSetup(IGetCreatureInfo unitInfo, IAttackResponse attackResponse)
        {
            m_Info = unitInfo;
            m_AnimateComp.SetAnimation(AnimateType.Attack);
            Attack(attackResponse); // TESTING
        }

        // Use ANIMATION's EVENT to take damage enemy and keep effect be execute simultaneously
        private void Attack(IAttackResponse attackResponser)
        {
            var currentState = m_Info.GetCurrentState();
            var attackRange =
                m_SkillComp.AttackPoints(currentState.midPos, currentState.direction, currentState.jumpStep);
            var attackPoints = attackRange as Vector3[] ?? attackRange.ToArray();
            // Check jumping boost
            if (m_EffectComp.UseJumpBoost())
                currentState.jumpStep += m_EffectComp.GetJumpBoost();
            m_AttackComp.Attack(attackPoints, this, currentState.jumpStep, m_Info.GetEnvironment());

            ShowAttackRange(attackPoints);
            attackResponser.AttackResponse();
        }

        private void ShowAttackRange(IEnumerable<Vector3> attackRange)
        {
            if (m_AttackPath is not null) m_AttackPath.AttackAt(attackRange);
        }

        public override int GetAttackDamage()
        {
            return m_CreatureData.CurrentDamage;
        }

        #endregion

        #region MOVE

        public void ConductCreatureMove(Vector3 currPos, int direction, ICreatureMove creature)
        {
            m_AnimateComp.MoveToTarget(currPos, direction, creature);
        }

        // public void ConductCreatureMove(Vector3 currPos, int direction, ICreatureMove creature)
        // {
        //     m_AnimateComp.MoveToTarget(currPos, direction, creature);
        // }

        #endregion

        #region SKILL

        public override IEnumerable<Skill_SO> GetSkills()
        {
            return m_SkillComp.GetSkills();
        }

        #endregion

        #region EFFECT

        public override EffectComp GetEffectComp()
        {
            return m_EffectComp;
        }

        #endregion

        #region ANIMATE COMPONENT

        public override void SetAnimation(AnimateType animateType, bool isTurnOn)
        {
            m_AnimateComp.SetAnimation(animateType);
        }

        #endregion

        #region GENERAL

        public override void ContributeCommands()
        {
            foreach (var command in m_CurrentStat.Commands)
                SavingSystemManager.Instance.StoreCurrencyAtBuildings(command.ToString(), m_CreatureData.Position);
        }

        public override void RefreshEntity()
        {
            // Set stats based on currentLevel
            m_CurrentStat = m_UnitStats[m_CreatureData.CurrentLevel];

            // Initiate entity data if it's new
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_CreatureData.EntityName);
            m_CreatureData.SkinAddress = inventoryItem.skinAddress[m_CreatureData.CurrentLevel];
            m_CreatureData.CreatureType = m_CurrentStat.CreatureType;
            if (m_CreatureData.CurrentHp <= 0)
            {
                m_CreatureData.CurrentHp = m_CurrentStat.HealthPoint;
                m_CreatureData.CurrentDamage = m_CurrentStat.Strengh;
            }

            // Retrieve entity data
            m_SkinComp.Init(m_CreatureData.SkinAddress, m_AnimateComp);
            m_HealthComp.Init(m_CurrentStat.HealthPoint, OnUnitDie, m_CreatureData);
            m_EffectComp.Init(this);
            m_SkillComp.Init(m_CreatureData.EntityName);
            OnUnitDie.AddListener(DieIndividualProcess);
            _isDie = false;

            // Check expand map
            // SavingSystemManager.Instance.OnCheckExpandMap.Invoke();
        }

        #endregion

        public UnitStats GetStats()
        {
            return m_CurrentStat;
        }
    }
}