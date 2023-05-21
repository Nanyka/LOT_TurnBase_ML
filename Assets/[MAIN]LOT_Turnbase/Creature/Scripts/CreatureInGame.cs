using System.Collections;
using System.Collections.Generic;
using LOT_Turnbase;
using UnityEngine;

namespace LOT_Turnbase
{
    // TODO replicate UnitMovement

    public class CreatureInGame : MonoBehaviour, IGetCreatureInfo
    {
        [Header("Unit Components")] [SerializeField]
        private Transform _rotatePart;

        [SerializeField] private Renderer _agentRenderer;
        [SerializeField] private CreatureEntity m_Entity;

        private FactionController m_FactionController;
        private Transform m_Transform;
        private Vector3 _defaultPos;
        private (Vector3 targetPos, int jumpCount, int overEnemy) _movement;
        private int _currentPower;
        private bool _isUsed;

        public void Init(CreatureData creatureData, FactionController faction)
        {
            m_Entity.Init(creatureData);
            m_FactionController = faction;
            m_FactionController.AddCreatureToFaction(this);
        }

        public void OnEnable()
        {
            m_Entity.OnUnitDie.AddListener(UnitDie);

            m_Transform = transform;
            _defaultPos = m_Transform.position;
        }

        public void MoveDirection(int moveDirection)
        {
            _movement = m_FactionController.GetMovementCalculator()
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
            m_FactionController.UnitMoved();
        }

        public void NewTurnReset(Material factionMaterial)
        {
            _isUsed = false;
            SetMaterial(factionMaterial);
        }

        public void ResetUnit()
        {
            m_Transform.position = _defaultPos;
            _movement.targetPos = _defaultPos;
            _isUsed = false;
            m_Entity.ResetEntity();
        }

        public void Attack()
        {
            m_Entity.Attack(this);
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

        public (string name, int health, int damage, int power) GetUnitInfo()
        {
            // return (name, m_Entity.GetCurrentHealth(), m_Entity.GetAttackDamage(), _movement.jumpCount);
            return (name, 0, 0, _movement.jumpCount);
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

        public void SetMaterial(Material setMaterial)
        {
            _agentRenderer.material = setMaterial;
        }

        public void SetUsedState()
        {
            _isUsed = true;
        }

        private void UnitDie()
        {
            m_FactionController.RemoveAgent(this);
            Destroy(gameObject, 1f);
        }

        #endregion
    }
}