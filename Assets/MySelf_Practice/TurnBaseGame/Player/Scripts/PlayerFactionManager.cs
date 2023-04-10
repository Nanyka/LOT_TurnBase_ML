using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFactionManager : MonoBehaviour
{
    [SerializeField] private EnvironmentInGame m_Environment;
    [SerializeField] private int m_Faction = 0;
    [SerializeField] private Material _factionMaterial;
    [SerializeField] private List<UnitMovement> _unitMovements = new(4);

    private Camera _camera;
    private int _layerMask = 1 << 3;
    private UnitMovement _currentUnit;
    private int _countMovedUnit;
    private Material _defaultMaterial;

    private void Start()
    {
        m_Environment.OnChangeFaction.AddListener(ToMyTurn);
        m_Environment.OnTouchSelection.AddListener(MoveToward);
        m_Environment.OnOneTeamWin.AddListener(EndGame);

        _camera = Camera.main;
        _defaultMaterial = _unitMovements[0].GetMaterial();

        MultiJumperKickOff();
    }

    #region ONE-TURN PIPELINE

    // Start game
    private void MultiJumperKickOff()
    {
        if (m_Faction == 0)
            m_Environment.KickOffEnvironment();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && m_Environment.GetCurrFaction() == m_Faction)
        {
            var moveRay = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(moveRay, out var moveHit, 100f, _layerMask))
                return;

            var pos = moveHit.point;
            SelectUnit(new Vector3(Mathf.RoundToInt(pos.x), 0f, Mathf.RoundToInt(pos.z)));
        }
    }

    private void ToMyTurn()
    {
        if (m_Environment.GetCurrFaction() != m_Faction)
            return;

        foreach (var unitMovement in _unitMovements)
            unitMovement.ResetUnit(_factionMaterial);

        _countMovedUnit = 0;
    }

    // Show unit selection && Show moving path when unit is still available
    private void SelectUnit(Vector3 unitPos)
    {
        var getUnitAtPos = GetUnitByPos(unitPos);
        if (getUnitAtPos == null) return;

        _currentUnit = getUnitAtPos;
        if (_currentUnit.IsAvailable())
            m_Environment.OnShowMovingPath.Invoke(unitPos);
        else
            Debug.Log("Highlight selected unit");
        
        UIManager.Instance.OnShowUnitInfo.Invoke(getUnitAtPos);
    }

    private void MoveToward(int direction)
    {
        //TODO Control player unit

        // Move unit along dedicated direction
        _currentUnit.MoveDirection(direction);

        // "Don't move" button

        // End turn

        // Debug.Log($"Check mouse pointer work or not: {unitPos}");
    }

    public void UnitMoved()
    {
        _countMovedUnit++;

        if (_countMovedUnit == _unitMovements.Count)
            EndTurn();
    }

    private void EndTurn()
    {
        // Attack nearby enemy
        int successAttacks = 0;
        foreach (var unit in _unitMovements)
        {
            if (unit.GetJumpStep() == 0)
                continue;

            var attackPoints = unit.GetAttackPoint();
            if (attackPoints == null)
                continue;

            foreach (var attackPoint in attackPoints)
            {
                if (m_Environment.CheckEnemy(attackPoint, m_Faction))
                {
                    var enemy = m_Environment.GetEnemyByPosition(attackPoint, m_Faction);
                    if (enemy.TryGetComponent(out UnitEntity enemyEntity))
                    {
                        enemyEntity.TakeDamage(unit.GetAttackDamage());
                    }
                    successAttacks++;
                }
            }
        }
        
        foreach (var unitMovement in _unitMovements)
            unitMovement.SetMaterial(_defaultMaterial);

        m_Environment.ChangeFaction();
        m_Environment.OnChangeFaction.Invoke();
    }

    private void EndGame(int winFaction)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region GET & SET

    public Collider GetPlatformCollider()
    {
        return m_Environment.GetPlatformCollider();
    }

    public MovementCalculator GetMovementCalculator()
    {
        return m_Environment.GetMovementCalculator();
    }

    private UnitMovement GetUnitByPos(Vector3 unitPos)
    {
        return _unitMovements.Find(x => Vector3.Distance(x.transform.position, unitPos) < Mathf.Epsilon);
    }

    #endregion
}