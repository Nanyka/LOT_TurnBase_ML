using System;
using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class CreatureInGame : MonoBehaviour, IGetEntityInfo, IShowInfo, IRemoveEntity, ICreatureMove,
        IAttackResponse, ICreatureType
    {
        [Header("Creature Components")] [SerializeField]
        protected Transform m_RotatePart;

        [SerializeField] protected CreatureEntity m_Entity;

        protected IFactionController m_FactionController;
        protected Transform m_Transform;
        private (Vector3 targetPos, int jumpCount, int overEnemy) _movement;
        private int _currentDirection;
        private int _currentPower;
        [SerializeField] private bool _isUsed;

        public virtual void Init(CreatureData creatureData, IFactionController playerFaction)
        {
            m_Entity.Init(creatureData);
            m_RotatePart.eulerAngles = creatureData.Rotation;
            m_FactionController = playerFaction;
            m_FactionController.AddCreatureToFaction(this);
            if (creatureData.CreatureType != CreatureType.ECOBOSS && GameFlowManager.Instance.GetEnvManager().CheckOutOfBoundary(creatureData.Position))
            {
                var availablePos = GameFlowManager.Instance.GetEnvManager().GetRandomAvailableTile();
                if (availablePos.x.Equals(float.NegativeInfinity) == false)
                    creatureData.Position = availablePos;
            }
            m_Transform.position = creatureData.Position;
            
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
            // Record creature action in BATTLE mode
            if (GameFlowManager.Instance.GameMode == GameMode.BATTLE)
            {
                var recordAction = new RecordAction
                {
                    Action = moveDirection,
                    // AtSecond = CountDownClock.GetBattleTime(),
                    AtPos = GetCurrentPosition(),
                    EntityType = EntityType.ENEMY
                };
            
                SavingSystemManager.Instance.OnRecordAction.Invoke(recordAction);
            }
            
            if (_isUsed && GameFlowManager.Instance.GameMode != GameMode.REPLAY) return; // Avoid double moving

            _currentDirection = moveDirection;
            _movement = m_FactionController.GetMovementInspector()
                .MovingPath(m_Transform.position, _currentDirection, 0, 0);

            MarkAsUsedThisTurn();
            CreatureStartMove(m_Transform.position, moveDirection);
        }

        protected virtual void CreatureStartMove(Vector3 currentPos, int direction)
        {
            MainUI.Instance.OnShowInfo.Invoke(this);
            m_Entity.ConductCreatureMove(currentPos, direction, this);

            if (_movement.jumpCount > 0)
                m_Entity.AttackSetup(this);
            
            m_Entity.TurnHealthSlider(false);
        }

        public virtual void CreatureEndMove()
        {
            m_Entity.UpdateTransform(_movement.targetPos, m_RotatePart.eulerAngles);
            m_Entity.TurnHealthSlider(true);
            
            // When double kills or more, take extra attack
            if (m_Entity.GetKillAccumulation() > 1)
            {
                m_Entity.GetAnimateComp().TriggerAttackAnim(m_Entity.GetKillAccumulation());
                m_Entity.ResetKillAccumulation();
            }
            else
                m_FactionController.WaitForCreature();
        }

        public void SkipThisTurn()
        {
            Debug.Log("TODO: Show SKIP TURN visual");
            MarkAsUsedThisTurn();
            m_FactionController.WaitForCreature();
        }

        public void NewTurnReset()
        {
            // m_Entity.GetEffectComp().EffectCountDown();
            _isUsed = m_Entity.GetEffectComp().CheckSkipTurn();
            _movement.jumpCount = 0;
            _movement.overEnemy = 0;
            // m_Entity.SetActiveMaterial();
            m_Entity.RefreshCreature();
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

        public (Entity, int) ShowInfo()
        {
            return (m_Entity, GetJumpStep());
        }

        public (Vector3 midPos, Vector3 direction, int jumpStep, FactionType faction) GetCurrentState()
        {
            return (_movement.targetPos,
                GameFlowManager.Instance.GetEnvManager().GetMovementInspector().DirectionTo(_currentDirection),
                _movement.jumpCount, m_FactionController.GetFaction());
        }

        public EntityData GetEntityData()
        {
            return m_Entity.GetData();
        }

        private int GetJumpStep()
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

        protected void MarkAsUsedThisTurn()
        {
            _isUsed = true;
        }

        protected void UnitDie(IAttackRelated killedByEntity)
        {
            if (GameFlowManager.Instance.GameMode == GameMode.ECONOMY)
            {
                // just contribute resource when it is killed by player faction
                if (killedByEntity.GetFaction() == FactionType.Player)
                    m_Entity.ContributeCommands();
            }
            
            // Contribute Exp
            if (killedByEntity.GetFaction() == FactionType.Player)
                SavingSystemManager.Instance.GainExp(m_Entity.GetStats().ExpReward);

            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this); // remove its domain
            m_FactionController.RemoveAgent(this);
            StartCoroutine(DieVisual());
        }

        private IEnumerator DieVisual()
        {
            yield return new WaitForSeconds(3f);
            gameObject.SetActive(false);
        }

        public void Remove(EnvironmentData environmentData)
        {
            if (m_Entity.GetData().EntityName.Equals("King"))
                return;

            if (m_FactionController.GetFaction() == FactionType.Player)
                environmentData.PlayerData.Remove((CreatureData)m_Entity.GetData());
            if (m_FactionController.GetFaction() == FactionType.Enemy)
                environmentData.EnemyData.Remove((CreatureData)m_Entity.GetData());
        }

        #endregion
    }
}