using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class CreatureEntity : Entity
    {
        [SerializeField] private UnitStats[] m_UnitStats;
        [SerializeField] private SkinComp m_SkinComp;
        [SerializeField] private HealthComp m_HealthComp;
        [SerializeField] private AttackComp m_AttackComp;
        [SerializeField] private SkillComp m_SkillComp;
        [SerializeField] private EffectComp m_EffectComp;
        [SerializeField] private LevelComp m_LevelComp;
        [SerializeField] private AttackPath m_AttackPath;
        [SerializeField] private AnimateComp m_AnimateComp;

        [SerializeField] private CreatureData m_CreatureData;
        private UnitStats m_CurrentStats;
        private IGetCreatureInfo m_Info;

        public void Init(CreatureData creatureData)
        {
            m_CreatureData = creatureData;
            RefreshEntity();
        }

        #region CREATURE DATA

        public override void UpdateTransform(Vector3 position, Vector3 rotation)
        {
            m_CreatureData.Position = position;
            m_CreatureData.Rotation = rotation;
            SavingSystemManager.Instance.OnSavePlayerEnvData.Invoke();
        }

        public override EntityData GetData()
        {
            return m_CreatureData;
        }

        public override CommandName GetCommand()
        {
            return m_CurrentStats.Command;
        }

        public override FactionType GetFaction()
        {
            return m_CreatureData.CreatureType;
        }

        public override int GetExpReward()
        {
            switch (hideFlags)
            {
                
            }
            
            return m_CurrentStats.ExpReward;
        }

        public override void CollectExp(int expAmount)
        {
            m_CreatureData.CurrentExp += expAmount;
            if (m_CreatureData.CurrentExp >= m_CurrentStats.ExpToLevelUp)
            {
                // Level up
            }
        }

        #endregion

        #region SKIN

        public void SetSkinMaterial(Material material)
        {
            m_SkinComp.SetMaterial(material);
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

        public override void DieCollect(Entity killedByEntity)
        {
            // Set animation and effect when entity die here
            m_AnimateComp.SetAnimation(AnimateType.Die, true);
        }

        #endregion

        #region ATTACK

        public override void AttackSetup(IGetCreatureInfo unitInfo)
        {
            Debug.Log($"{gameObject} attack");
            m_Info = unitInfo;
            m_AnimateComp.SetAnimation(AnimateType.Attack,true);
            Attack(); // TESTING
        }
        
        // Use ANIMATION's EVENT to take damage enemy and keep effect be execute simultaneously
        public void Attack()
        {
            var currentState = m_Info.GetCurrentState();
            var attackRange =
                m_SkillComp.AttackPoints(currentState.midPos, currentState.direction, currentState.jumpStep);
            var attackPoints = attackRange as Vector3[] ?? attackRange.ToArray();
            m_AttackComp.Attack(attackPoints, this, m_CreatureData.CurrentDamage , m_Info.GetEnvironment());

            ShowAttackRange(attackPoints);
            m_EffectComp.AttackVFX(currentState.jumpStep);
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

        #region SKILL

        public override IEnumerable<Skill_SO> GetSkills()
        {
            return m_SkillComp.GetSkills();
        }

        #endregion

        #region ANIMATE COMPONENT

        public override void SetAnimation(AnimateType animation, bool isTurnOn)
        {
            m_AnimateComp.SetAnimation(animation, isTurnOn);
        }

        #endregion

        #region GENERAL

        public override void RefreshEntity()
        {
            // Set stats based on currentLevel
            m_CurrentStats = m_UnitStats[m_CreatureData.CurrentLevel];
            
            // Initiate entity data if it's new
            var inventoryItem = SavingSystemManager.Instance.GetInventoryItemByName(m_CreatureData.EntityName);
            m_CreatureData.SkinAddress = inventoryItem.skinAddress[m_CreatureData.CurrentLevel];
            if (m_CreatureData.CurrentHp <= 0)
            {
                m_CreatureData.CurrentHp = m_CurrentStats.HealthPoint;
                m_CreatureData.CurrentDamage = m_CurrentStats.Strengh;
            }
            
            // Retrieve entity data
            m_Transform.position = m_CreatureData.Position;
            m_Transform.eulerAngles = m_CreatureData.Rotation;
            m_SkinComp.Initiate(m_CreatureData.SkinAddress, m_AnimateComp);
            m_HealthComp.Init(m_CurrentStats.HealthPoint, OnUnitDie, m_CreatureData);
            OnUnitDie.AddListener(DieCollect);
        }

        #endregion

    }
}