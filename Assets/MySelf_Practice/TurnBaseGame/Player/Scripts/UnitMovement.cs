using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour, IGetUnitInfo
{
    [SerializeField] private PlayerFactionManager m_FactionManager;

    [Header("Unit Components")]
    [SerializeField] private Transform _rotatePart;
    [SerializeField] private MeshRenderer _agentRenderer;
    [SerializeField] private UnitEntity _unitEntity;

    private Transform m_Transform;
    private (Vector3 targetPos, int jumpCount, int overEnemy) _movement;
    private int _currentPower;
    private bool _isUsed;

    private void Start()
    {
        m_Transform = transform;
    }

    public void MoveDirection(int moveDirection)
    {
        _movement = m_FactionManager.GetMovementCalculator()
            .MovingPath(m_Transform.position, moveDirection, 0, 0);

        // Change agent direction before the agent jump to the new position
        if (_movement.targetPos != m_Transform.position)
            _rotatePart.forward = _movement.targetPos - m_Transform.position;

        _isUsed = true;
        StartCoroutine(MoveOverTime(_movement.targetPos));
    }
    
    private IEnumerator MoveOverTime(Vector3 targetPos)
    {
        while (transform.position != targetPos)
        {
            m_Transform.position = Vector3.MoveTowards(transform.position, targetPos, 10f * Time.deltaTime);
            yield return null;
        }

        m_FactionManager.UnitMoved();
    }

    public void ResetUnit(Material factionMaterial)
    {
        _isUsed = false;
        SetMaterial(factionMaterial);
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

    public (string name, int health, int damage, int power) GetUnitInfo()
    {
        return (name ,3, 1, _movement.jumpCount);
    }

    public int GetJumpStep()
    {
        return _movement.jumpCount;
    }

    public IEnumerable<Vector3> GetAttackPoint()
    {
        return _unitEntity.GetAttackPoint(m_Transform.position,_rotatePart.forward, _movement.jumpCount);
    }

    public int GetAttackDamage()
    {
        return _unitEntity.GetAttackDamage();
    }
    
    public void SetMaterial(Material setMaterial)
    {
        _agentRenderer.material = setMaterial;
    }

    #endregion
}