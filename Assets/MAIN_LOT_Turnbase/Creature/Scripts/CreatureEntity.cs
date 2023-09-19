using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class CreatureEntity : Entity, IStatsProvider<CreatureStats>
    {
        [SerializeField] private Transform m_RotatePart;
        [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
        [SerializeField] private SkillComp m_SkillComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private AnimateComp m_AnimateComp;

        private List<CreatureStats> m_CreatureStats;
        private CreatureData m_CreatureData;
        private CreatureStats m_CurrentStat;
        private IEnumerable<Vector3> attackRange;
        private int _currentJumpStep;
        private bool _isDie;

        public void Init(CreatureData creatureData)
        {
            m_CreatureData = creatureData;

            SavingSystemManager.Instance.OnCreatureUpgrade.AddListener(CreatureUpgrade);

            RefreshEntity();
        }

        // Remove all listener when entity completed die process
        private void OnDisable()
        {
            OnUnitDie.RemoveAllListeners();
        }

        #region CREATURE DATA

        public override void Relocate(Vector3 position)
        {
            m_Transform.position = position;
        }

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            m_Transform.position = position;
            m_RotatePart.eulerAngles = rotation;

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

        public int GetUpgradeCost()
        {
            return m_CurrentStat.CostToLevelUp;
        }

        private void CreatureUpgrade(string creatureId)
        {
            if (creatureId.Equals(m_CreatureData.EntityName) == false)
                return;

            // Level up
            m_CreatureData.CurrentLevel++;

            // Reset stats and appearance
            m_CurrentStat = m_CreatureStats[m_CreatureData.CurrentLevel];
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_CreatureData.EntityName);
            m_CreatureData.SkinAddress = inventoryItem.skinAddress[m_CreatureData.CurrentLevel];
            m_CreatureData.CurrentDamage = m_CurrentStat.Strength;
            m_SkinComp.Init(m_CreatureData.SkinAddress, m_AnimateComp);
        }

        public bool CheckEntityDie()
        {
            return _isDie;
        }

        public bool CheckMaxLevel()
        {
            return m_CreatureData.CurrentLevel >= m_CreatureStats.Count() - 1;
        }

        #endregion

        #region SKIN

        public override SkinComp GetSkin()
        {
            return m_SkinComp;
        }

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
            if (m_CreatureData.EntityName.Equals("King") && GameFlowManager.Instance.GameMode == GameMode.ECONOMY)
                return;
                
            m_HealthComp.TakeDamage(damage, m_CreatureData, fromEntity);
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public override int GetCurrentHealth()
        {
            return m_CreatureData.CurrentHp;
        }

        public override void DieIndividualProcess(Entity killedByEntity)
        {
            _isDie = true;

            // Set animation and effect when entity die here
            m_AnimateComp.SetAnimation(AnimateType.Die);
        }

        public void TurnHealthSlider(bool isOn)
        {
            m_HealthComp.TurnHealthSlider(isOn);
        }

        #endregion

        #region ATTACK

        public override void AttackSetup(IGetEntityInfo unitInfo, IAttackResponse attackResponse) { }

        public void AttackSetup(IGetEntityInfo unitInfo)
        {
            var currentJump = unitInfo.GetCurrentState();

            // Check jumping boost
            if (m_EffectComp.UseJumpBoost())
                currentJump.jumpStep += m_EffectComp.GetJumpBoost();

            // Adjust by current level
            currentJump.jumpStep = currentJump.jumpStep < m_CreatureData.CurrentLevel + 1
                ? currentJump.jumpStep
                : m_CreatureData.CurrentLevel + 1;

            // Adjust by amount of skills
            currentJump.jumpStep = currentJump.jumpStep < m_SkillComp.GetSkillAmount()
                ? currentJump.jumpStep
                : m_SkillComp.GetSkillAmount();

            _currentJumpStep = currentJump.jumpStep;
            
            attackRange = m_SkillComp.AttackPoints(currentJump.midPos, currentJump.direction, currentJump.jumpStep);
            ShowAttackRange(attackRange);
        }
        
        // Use ANIMATION's EVENT to take damage enemy and keep effect be execute simultaneously
        public void Attack(int attackPathIndex)
        {
            m_AttackComp.Attack(attackRange.ElementAt(attackPathIndex), this, _currentJumpStep);
        }

        private void ShowAttackRange(IEnumerable<Vector3> attackRange)
        {
            // if (m_AttackPath is not null) m_AttackPath.AttackAt(attackRange);
            GameFlowManager.Instance.AskForShowingAttackPath(attackRange);
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

        public int GetJumpBoost()
        {
            return m_EffectComp.GetJumpBoost();
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
            if (m_CurrentStat.Commands == null || m_CreatureStats.Count == 0)
                return;
            
            foreach (var command in m_CurrentStat.Commands)
                SavingSystemManager.Instance.StoreCurrencyAtBuildings(command.ToString(), m_CreatureData.Position);
        }

        public override void RefreshEntity()
        {
            // Set stats based on currentLevel
            var creatureLevel = SavingSystemManager.Instance.GetInventoryLevel(m_CreatureData.EntityName);
            m_CreatureData.CurrentLevel = creatureLevel == 0 ? m_CreatureData.CurrentLevel : creatureLevel;
            var inventory = SavingSystemManager.Instance.GetInventoryItemByName(m_CreatureData.EntityName);
            m_CreatureStats = inventory.creatureStats;
            m_CurrentStat = m_CreatureStats[m_CreatureData.CurrentLevel];

            // Initiate entity data if it's new
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_CreatureData.EntityName);
            m_CreatureData.SkinAddress =
                inventoryItem.skinAddress[
                    Mathf.Clamp(m_CreatureData.CurrentLevel, 0, inventoryItem.skinAddress.Count - 1)];
            m_CreatureData.CreatureType = m_CurrentStat.CreatureType;
            if (m_CreatureData.CurrentHp <= 0)
            {
                m_CreatureData.CurrentHp = m_CurrentStat.HealthPoint;
                m_CreatureData.CurrentDamage = m_CurrentStat.Strength;
            }
            else
                m_CreatureData.CurrentHp = m_CreatureData.CurrentHp < m_CurrentStat.HealthPoint
                    ? m_CreatureData.CurrentHp
                    : m_CurrentStat.HealthPoint;

            // Retrieve entity data
            if (m_CreatureData.EntityName.Equals("King") && GameFlowManager.Instance.GameMode != GameMode.ECONOMY)
                m_CreatureData.CurrentHp = m_CurrentStat.HealthPoint;
            m_SkinComp.Init(m_CreatureData.SkinAddress, m_AnimateComp);
            m_HealthComp.Init(m_CurrentStat.HealthPoint, OnUnitDie, m_CreatureData);
            m_EffectComp.Init(this);
            m_SkillComp.Init(m_CreatureData.EntityName);
            OnUnitDie.AddListener(DieIndividualProcess);
            _isDie = false;
        }

        #endregion

        public CreatureStats GetStats()
        {
            return m_CurrentStat;
        }

        public IEnumerable<Vector3> GetAttackRange()
        {
            return attackRange;
        }
    }
}