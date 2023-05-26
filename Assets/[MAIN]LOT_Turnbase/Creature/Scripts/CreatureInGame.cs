using System.Collections;
using System.Collections.Generic;
using LOT_Turnbase;
using UnityEngine;

namespace LOT_Turnbase
{
    // TODO replicate UnitMovement

    public class CreatureInGame : MonoBehaviour, IGetCreatureInfo
    {
        [Header("Creature Components")] 
        [SerializeField] protected Transform _rotatePart;
        [SerializeField] private Renderer _agentRenderer;
        [SerializeField] protected CreatureEntity m_Entity;

        protected IFactionController m_FactionController;
        protected Transform m_Transform;
        private (Vector3 targetPos, int jumpCount, int overEnemy) _movement;
        private int _currentPower;
        private bool _isUsed;

        public void Init(CreatureData creatureData, IFactionController playerFaction)
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
            _movement = m_FactionController.GetMovementInspector()
                .MovingPath(m_Transform.position, moveDirection, 0, 0);

            // Change agent direction before the agent jump to the new position
            if (_movement.targetPos != m_Transform.position)
                _rotatePart.forward = _movement.targetPos - m_Transform.position;

            _isUsed = true;
            m_Entity.SetAnimation(AnimateType.Walk,true);
            StartCoroutine(MoveOverTime(_movement.targetPos));
        }

        private IEnumerator MoveOverTime(Vector3 targetPos)
        {
            while (transform.position != targetPos)
            {
                m_Transform.position = Vector3.MoveTowards(transform.position, targetPos, 5f * Time.deltaTime);
                yield return null;
            }

            m_Entity.SetAnimation(AnimateType.Walk,false);
            m_FactionController.WaitForCreature();
        }

        public void NewTurnReset(Material factionMaterial)
        {
            _isUsed = false;
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

        public Material GetMaterial()
        {
            return _agentRenderer.material;
        }

        public Vector3 GetCurrentPosition()
        {
            return m_Transform.position;
        }

        public (string name, int health, int damage, int power) InfoToShow()
        {
            return (name, m_Entity.GetCurrentHealth(), m_Entity.GetAttackDamage(), _movement.jumpCount);
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
            _agentRenderer.material = setMaterial;
        }

        public void MarkAsUsedThisTurn()
        {
            _isUsed = true;
        }

        protected virtual void UnitDie()
        {
            m_FactionController.RemoveAgent(this);
            Destroy(gameObject, 1f);
        }

        public void ResetMoveState(Material factionMaterial)
        {
            _isUsed = false;
            _agentRenderer.material = factionMaterial;
        }

        #endregion
    }
}