using System;
using System.Collections;
using UnityEngine;

public class UnitMovement : MonoBehaviour, IGetUnitInfo
{
    [SerializeField] private PlayerFactionManager m_FactionManager;

    [Header("Unit Components")]
    [SerializeField] private Transform _rotatePart;
    [SerializeField] private MeshRenderer _agentRenderer;
    [SerializeField] private UnitEntity _unitEntity;

    private Transform m_Transform;
    private Vector3 _defaultPos;
    private (Vector3 targetPos, int jumpCount, int overEnemy) _movement;
    private int _currentPower;
    private bool _isUsed;

    public void OnEnable()
    {
        _unitEntity.OnUnitDie.AddListener(UnitDie);
        
        m_Transform = transform;
        _defaultPos = m_Transform.position;
    }

    public void MoveDirection(int moveDirection)
    {
        _movement = m_FactionManager.GetMovementCalculator()
            .MovingPath(m_Transform.position, moveDirection, 0, 0);

        // Change agent direction before the agent jump to the new position
        if (_movement.targetPos != m_Transform.position)
            _rotatePart.forward = _movement.targetPos - m_Transform.position;

        _isUsed = true;
        _unitEntity.SeWalkAnimation(true);
        StartCoroutine(MoveOverTime(_movement.targetPos));
    }
    
    private IEnumerator MoveOverTime(Vector3 targetPos)
    {
        while (transform.position != targetPos)
        {
            m_Transform.position = Vector3.MoveTowards(transform.position, targetPos, 5f * Time.deltaTime);
            yield return null;
        }

        _unitEntity.SeWalkAnimation(false);
        m_FactionManager.UnitMoved();
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
        _unitEntity.ResetEntity();
    }

    public void Attack()
    {
        _unitEntity.Attack(this);
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
        return (name , _unitEntity.GetCurrentHealth(), _unitEntity.GetAttackDamage(), _movement.jumpCount);
    }

    public (Vector3 midPos, Vector3 direction, int jumpStep, int faction) GetCurrentState()
    {
        return (m_Transform.position, _rotatePart.forward, _movement.jumpCount, m_FactionManager.GetFaction());
    }

    public EnvironmentController GetEnvironment()
    {
        return m_FactionManager.GetEnvironment();
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
        m_FactionManager.RemoveAgent(this);
        Destroy(gameObject,1f);
    }

    #endregion
}