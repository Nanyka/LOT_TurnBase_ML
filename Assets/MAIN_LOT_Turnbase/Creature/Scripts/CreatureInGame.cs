using System;
using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class CreatureInGame : MonoBehaviour, IGetCreatureInfo, IShowInfo, IRemoveEntity, ICreatureMove, IAttackResponse
    {
        [FormerlySerializedAs("_rotatePart")]
        [Header("Creature Components")] 
        [SerializeField] protected Transform _tranformPart;
        [SerializeField] protected CreatureEntity m_Entity;

        protected IFactionController m_FactionController;
        protected Transform m_Transform;
        private (Vector3 targetPos, int jumpCount, int overEnemy) _movement;
        private int _currentPower;
        private bool _isUsed;

        public virtual void Init(CreatureData creatureData, IFactionController playerFaction)
        {
            m_Entity.Init(creatureData);
            m_Transform.position = creatureData.Position;
            _tranformPart.eulerAngles = creatureData.Rotation;
            
            m_FactionController = playerFaction;
            m_FactionController.AddCreatureToFaction(this);
            MarkAsUsedThisTurn();
        }

        public virtual void OnEnable()
        {
            m_Entity.OnUnitDie.AddListener(UnitDie);
            m_Transform = transform;
        }

        private void OnDisable()
        {
            m_Entity.OnUnitDie.RemoveListener(UnitDie);
        }

        public void MoveDirection(int moveDirection)
        {
            if (_isUsed) return; // Avoid double moving

            _movement = m_FactionController.GetMovementInspector()
                .MovingPath(m_Transform.position, moveDirection, 0, 0);

            MarkAsUsedThisTurn();
            CreatureStartMove(m_Transform.position,moveDirection);
            // StartCoroutine(MoveOverTime(_movement.targetPos));
        }
        
        public void CreatureStartMove(Vector3 currentPos, int direction)
        {
            m_Entity.ConductCreatureMove(currentPos,direction, this);
        }

        public virtual void CreatureEndMove()
        {
            m_Entity.UpdateTransform(_movement.targetPos, _tranformPart.eulerAngles);
            if (GetJumpStep() > 0)
                Attack();
            else
                m_FactionController.WaitForCreature();
        }

        // private IEnumerator MoveOverTime(Vector3 targetPos)
        // {
        //     m_Entity.SetAnimation(AnimateType.Walk, true);
        //     m_Entity.UpdateTransform(_movement.targetPos, _tranformPart.eulerAngles);
        //     while (m_Transform.position != targetPos)
        //     {
        //         m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, 2f * Time.deltaTime);
        //         yield return null;
        //     }
        //
        //     m_Entity.SetAnimation(AnimateType.Walk, false);
        //     m_FactionController.WaitForCreature();
        // }

        public void NewTurnReset()
        {
            _isUsed = false;
            _movement.jumpCount = 0;
            _movement.overEnemy = 0;
            m_Entity.SetActiveMaterial();
        }

        private void Attack()
        {
            m_Entity.AttackSetup(this, this);
        }

        public virtual void AttackResponse()
        {
            m_FactionController.WaitForCreature();
        }

        #region GET & SET

        public bool IsAvailable()
        {
            return !_isUsed;
        }

        public Vector3 GetCurrentPosition()
        {
            return m_Transform.position;
        }

        public string ShowInfo()
        {
            var data = (CreatureData)m_Entity.GetData();
            return
                $"{data.EntityName}\nHp:{data.CurrentHp}\nDamage:{data.CurrentDamage}\nJumpCount:{_movement.jumpCount}";
        }

        public (Vector3 midPos, Vector3 direction, int jumpStep, FactionType faction) GetCurrentState()
        {
            return (m_Transform.position, _tranformPart.forward, _movement.jumpCount, m_FactionController.GetFaction());
        }

        public EntityData GetEntityData()
        {
            return m_Entity.GetData();
        }
        
        public EnvironmentManager GetEnvironment()
        {
            return m_FactionController.GetEnvironment();
        }

        public int GetJumpStep()
        {
            return _movement.jumpCount;
        }

        public bool CheckUsedThisTurn()
        {
            return _isUsed;
        }

        public void SetDisableMaterial()
        {
            m_Entity.SetDisableMaterial();
        }

        public void MarkAsUsedThisTurn()
        {
            _isUsed = true;
        }

        protected void UnitDie(Entity killedByEntity)
        {
            if (GameFlowManager.Instance.IsEcoMode)
            {
                // just contribute resource when it is killed by player faction
                if (killedByEntity.GetFaction() == FactionType.Player)
                    m_Entity.ContributeCommands();
            }

            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this); // remove its domain
            m_FactionController.RemoveAgent(this);
            StartCoroutine(DieVisual());
        }

        private IEnumerator DieVisual()
        {
            yield return new WaitForSeconds(2f);
            gameObject.SetActive(false);
        }

        public void Remove(EnvironmentData environmentData)
        {
            if (m_FactionController.GetFaction() == FactionType.Player)
                environmentData.PlayerData.Remove((CreatureData) m_Entity.GetData());
            if (m_FactionController.GetFaction() == FactionType.Enemy)
                environmentData.EnemyData.Remove((CreatureData) m_Entity.GetData());
        }

        // public void ResetMoveState()
        // {
        //     _isUsed = false;
        //     m_Entity.SetActiveMaterial();
        // }

        #endregion
    }
}