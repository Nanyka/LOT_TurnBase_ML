using System;
using System.Collections.Generic;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class CharacterEntity : Entity, ISpecialAttackReceiver, ITroopAssembly
    {
        public Vector3 _assemblyPoint { get; set; }

        // control components
        [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private MovementComp m_MovementComp;
        [SerializeField] private AOEAnimateComp m_AnimateComp;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private EnemyBrainComp m_Brain;

        private ISkillMonitor m_SkillMonitor;
        
        // loaded data
        private List<CreatureStats> m_CreatureStats;
        
        // in-game data
        private CreatureData m_CreatureData;
        private CreatureStats m_CurrentStat;
        private ICharacterAttack _currentAttack;
        [SerializeField] private int _attackIndex;
        private int _killAccumulation;
        private bool _isDie;

        #region CREATURE DATA

        private void Awake()
        {
            m_SkillMonitor = GetComponent<ISkillMonitor>();
        }

        public void Init(CreatureData creatureData)
        {
            m_CreatureData = creatureData;
            m_Transform = transform;
            m_Transform.position = m_CreatureData.Position;

            RefreshEntity();
        }

        public override void Relocate(Vector3 position) { }

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            throw new System.NotImplementedException();
        }

        public override EntityData GetData()
        {
            return m_CreatureData;
        }

        public override FactionType GetFaction()
        {
            throw new System.NotImplementedException();
        }

        // public override void GainGoldValue()
        // {
        //     throw new System.NotImplementedException();
        // }

        public void SetAssemblyPoint(Vector3 assemblyPoint)
        {
            _assemblyPoint = assemblyPoint;
            m_Brain.AddBeliefs("HaveJustWentOut");
        }

        public CreatureStats GetStats()
        {
            return m_CurrentStat;
        }

        #endregion

        #region SKIN

        public override SkinComp GetSkin()
        {
            return m_SkinComp;
        }

        private void SetActiveMaterial()
        {
            m_SkinComp.SetActiveMaterial();
        }

        #endregion

        #region HEALTH

        public void TakeDamage(int damage, IAttackRelated fromEntity)
        {
            throw new System.NotImplementedException();
        }

        protected virtual void DieIndividualProcess(IAttackRelated killedByEntity)
        {
            _isDie = true;

            // Set animation and effect when entity die here
            m_AnimateComp.SetAnimation(AnimateType.Die);
        }

        #endregion

        #region MOVEMENT

        public void MoveTowards(Vector3 destination, IProcessUpdate processUpdate)
        {
            if (Vector3.Distance(m_Transform.position, destination) < m_MovementComp.GetStopDistance())
            {
                processUpdate.StopProcess();
                return;
            }
            
            m_MovementComp.MoveTo(destination, processUpdate, m_AnimateComp);
            // m_AnimateComp.SetAnimation(AnimateType.Walk, true);
        }

        public void StopMoving()
        {
            m_AnimateComp.SetAnimation(AnimateType.Walk, false);
        }

        public float GetStopDistance()
        {
            return m_MovementComp.GetStopDistance();
        }

        #endregion

        #region ATTACK

        public virtual void StartAttack(ICharacterAttack attack)
        {
            _currentAttack = attack;
            m_AnimateComp.TriggerAttackAnim(_attackIndex);
            m_SkillMonitor.ResetPowerBar();
        }

        // TODO: Refactor it by splitting it into a separated component
        public override void SuccessAttack(GameObject target)
        {
            _currentAttack.ExecuteAttack(target);
            m_SkillMonitor.PowerUp();
        }

        #endregion

        #region SKILL
        
        // Is set from the FactoryComp
        public void EnablePowerBar(int index)
        {
            m_SkillMonitor.SetSpecialAttack(index);
        }

        // PowerComp use this function to reset attackIndex
        public void SetAttackIndex(int index)
        {
            _attackIndex = index;
        }

        #endregion

        #region EFFECT

        // public override EffectComp GetEffectComp()
        // {
        //     throw new System.NotImplementedException();
        // }

        #endregion

        #region GENERAL

        private void RefreshEntity()
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
                inventoryItem.skinAddress[Mathf.Clamp(m_CreatureData.CurrentLevel, 0, inventoryItem.skinAddress.Count - 1)];
            m_CreatureData.CreatureType = m_CurrentStat.CreatureType;
            m_CreatureData.CurrentExp = 0;
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
            OnUnitDie.AddListener(DieIndividualProcess);
            _isDie = false;
        }

        public void RefreshCreature()
        {
            m_EffectComp.EffectCountDown();
            SetActiveMaterial();
            _killAccumulation = 0;
        }

        #endregion
    }

    public interface ITroopAssembly
    {
        public void SetAssemblyPoint(Vector3 assemblyPoint);
    }
    
    public interface ISpecialAttackReceiver
    {
        public void EnablePowerBar(int index);
        public void SetAttackIndex(int index);
    }
}