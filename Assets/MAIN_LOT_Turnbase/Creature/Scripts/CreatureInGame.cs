using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;

namespace JumpeeIsland
{
    // TODO replicate UnitMovement

    public class CreatureInGame : MonoBehaviour, IGetCreatureInfo, IShowInfo, IRemoveEntity
    {
        [Header("Creature Components")] [SerializeField]
        protected Transform _rotatePart;

        // [SerializeField] private Renderer _agentRenderer;
        [SerializeField] protected CreatureEntity m_Entity;

        protected IFactionController m_FactionController;
        protected Transform m_Transform;
        private (Vector3 targetPos, int jumpCount, int overEnemy) _movement;
        private int _currentPower;
        private bool _isUsed;

        public virtual void Init(CreatureData creatureData, IFactionController playerFaction)
        {
            m_Entity.Init(creatureData);
            m_FactionController = playerFaction;
            m_FactionController.AddCreatureToFaction(this);
        }

        public void OnEnable()
        {
            m_Entity.OnUnitDie.AddListener(UnitDie);
            m_Transform = transform;
        }

        public void MoveDirection(int moveDirection)
        {
            if (_isUsed) return; // Avoid double moving

            _movement = m_FactionController.GetMovementInspector()
                .MovingPath(m_Transform.position, moveDirection, 0, 0);

            // Change agent direction before the agent jump to the new position
            if (_movement.targetPos != m_Transform.position)
                _rotatePart.forward = _movement.targetPos - m_Transform.position;

            _isUsed = true;
            m_Entity.SetAnimation(AnimateType.Walk, true);
            m_Entity.UpdateTransform(_movement.targetPos, _rotatePart.eulerAngles);
            StartCoroutine(MoveOverTime(_movement.targetPos));
        }

        private IEnumerator MoveOverTime(Vector3 targetPos)
        {
            while (m_Transform.position != targetPos)
            {
                m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, 5f * Time.deltaTime);
                yield return null;
            }

            m_Entity.SetAnimation(AnimateType.Walk, false);
            m_FactionController.WaitForCreature();
        }

        public void NewTurnReset(Material factionMaterial)
        {
            _isUsed = false;
            _movement.jumpCount = 0;
            _movement.overEnemy = 0;
            SetMaterial(factionMaterial);
        }

        public void Attack()
        {
            m_Entity.AttackSetup(this);
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
            return (m_Transform.position, _rotatePart.forward, _movement.jumpCount, m_FactionController.GetFaction());
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

        public void SetMaterial(Material setMaterial)
        {
            m_Entity.SetSkinMaterial(setMaterial);
        }

        public void MarkAsUsedThisTurn()
        {
            _isUsed = true;
        }

        protected virtual void UnitDie(Entity killedByEntity)
        {
            // just contribute resource when it is killed by player faction
            if (killedByEntity.GetFaction() == FactionType.Player)
                SavingSystemManager.Instance.OnContributeCommand.Invoke(m_Entity.GetCommand());

            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);
            m_FactionController.RemoveAgent(this);
            StartCoroutine(DieVisual());
        }

        private IEnumerator DieVisual()
        {
            // Collect currencies
            // VFX
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }

        public void Remove(EnvironmentData environmentData)
        {
            if (m_FactionController.GetFaction() == FactionType.Player)
                environmentData.PlayerData.Remove((CreatureData) m_Entity.GetData());
            if (m_FactionController.GetFaction() == FactionType.Enemy)
                environmentData.EnemyData.Remove((CreatureData) m_Entity.GetData());
        }

        public void ResetMoveState(Material factionMaterial)
        {
            _isUsed = false;
            m_Entity.SetSkinMaterial(factionMaterial);
        }

        #endregion
    }
}