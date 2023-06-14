using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JumpeeIsland
{
    public class PlayerFactionController : MonoBehaviour, IFactionController
    {
        [SerializeField] private FactionType m_Faction = FactionType.Player;
        [SerializeField] private Material _factionMaterial;
        [SerializeField] private Material _defaultMaterial;

        private List<CreatureInGame> _creatures = new();
        private EnvironmentManager m_Environment;
        private Camera _camera;
        private int _layerMask = 1 << 7;
        private CreatureInGame _currentUnit;
        private int _countMovedUnit;

        // Add creature to this faction manager from it's Init()
        public void AddCreatureToFaction(CreatureInGame creature)
        {
            _creatures.Add(creature);
        }

        public void Init()
        {
            m_Environment = GameFlowManager.Instance.GetEnvManager();
            m_Environment.OnChangeFaction.AddListener(ToMyTurn);
            m_Environment.OnTouchSelection.AddListener(MoveToward);
            MainUI.Instance.OnClickIdleButton.AddListener(SetCurrentUnitIdle);

            _camera = Camera.main;
        }

        #region ONE-TURN PIPELINE

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (m_Environment.GetCurrFaction() != m_Faction)
                    return;

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

            foreach (var unitMovement in _creatures)
                unitMovement.NewTurnReset(_factionMaterial);

            KickOffNewTurn();
        }

        public void KickOffNewTurn()
        {
            _countMovedUnit = 0;
        }

        // Show unit selection && Show moving path when unit is still available
        private void SelectUnit(Vector3 unitPos)
        {
            var getUnitAtPos = GetUnitByPos(unitPos);
            if (getUnitAtPos == null) return;

            HighlightSelectedUnit(getUnitAtPos);
        }

        private void HighlightSelectedUnit(CreatureInGame getUnitAtPos)
        {
            _currentUnit = getUnitAtPos;

            if (_currentUnit.IsAvailable())
                m_Environment.OnShowMovingPath.Invoke(_currentUnit.GetCurrentPosition());
            else
                m_Environment.OnHighlightUnit.Invoke(_currentUnit.GetCurrentPosition());

            MainUI.Instance.OnShowInfo.Invoke(getUnitAtPos);
        }

        private void MoveToward(int direction)
        {
            if (m_Environment.GetCurrFaction() != m_Faction)
                return;

            // Move unit along dedicated direction
            _currentUnit.MoveDirection(direction);
        }

        public void WaitForCreature()
        {
            _countMovedUnit++;

            if (_countMovedUnit == _creatures.Count)
                EndTurn();
            else
            {
                foreach (var unit in _creatures)
                {
                    if (unit.IsAvailable())
                    {
                        HighlightSelectedUnit(unit);
                        break;
                    }
                }
            }
        }

        private void SetCurrentUnitIdle()
        {
            if (_currentUnit == null || !_currentUnit.IsAvailable())
                return;
            _currentUnit.MarkAsUsedThisTurn();
            WaitForCreature();
        }

        private void EndTurn()
        {
            // Attack nearby enemy
            foreach (var unit in _creatures)
            {
                if (unit.GetJumpStep() == 0)
                    continue;

                unit.Attack();
            }

            foreach (var unitMovement in _creatures)
                unitMovement.SetMaterial(_defaultMaterial);

            StartCoroutine(WaitForChangeFaction());
        }

        private IEnumerator WaitForChangeFaction()
        {
            yield return new WaitForSeconds(1f);
            m_Environment.ChangeFaction();
        }

        #endregion

        #region GET & SET

        public EnvironmentManager GetEnvironment()
        {
            return m_Environment;
        }

        public FactionType GetFaction()
        {
            return m_Faction;
        }

        public MovementInspector GetMovementInspector()
        {
            return m_Environment.GetMovementInspector();
        }

        private CreatureInGame GetUnitByPos(Vector3 unitPos)
        {
            return _creatures.Find(x => Vector3.Distance(x.transform.position, unitPos) < Mathf.Epsilon);
        }

        public void RemoveAgent(CreatureInGame unitMovement)
        {
            m_Environment.RemoveObject(unitMovement.gameObject, m_Faction);
            _creatures.Remove(unitMovement);
        }

        public void ResetData()
        {
            _creatures = new List<CreatureInGame>();
        }

        #endregion
    }
}