using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JumpeeIsland
{
    public class PlayerFactionController : MonoBehaviour, IFactionController
    {
        public bool _isAutomation;

        [SerializeField] [ShowIf("@_isAutomation == true")]
        private PlayerNpcController m_PlayerNpcController;

        private FactionType m_Faction = FactionType.Player;
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
            GameFlowManager.Instance.OnChangeAutomationMode.AddListener(ChangeAutomation);

            _camera = Camera.main;
        }

        private void ChangeAutomation(bool isAutomation)
        {
            _isAutomation = isAutomation;
        }

        #region ONE-TURN PIPELINE

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (MainUI.Instance.IsInteractable == false || PointingChecker.IsPointerOverUIObject())
                    return;

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

            if (_creatures.Count == 0)
            {
                EndTurn();
                return;
            }

            StopAllCoroutines();
            foreach (var creature in _creatures)
                creature.NewTurnReset();

            KickOffNewTurn();
        }

        public void KickOffNewTurn()
        {
            _countMovedUnit = _creatures.Count(t => t.CheckUsedThisTurn());
            if (_countMovedUnit == _creatures.Count)
                EndTurn();
            else
                SelectUnit(_creatures[0].GetCurrentPosition());
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
                m_Environment.OnHighlightUnit.Invoke(_currentUnit
                    .GetCurrentPosition()); // TODO highlight unavailable creature

            GameFlowManager.Instance.OnSelectEntity.Invoke(_currentUnit.GetEntityData());
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
            _countMovedUnit = _creatures.Count(t => t.CheckUsedThisTurn());

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
            _currentUnit.SkipThisTurn();
        }

        private void EndTurn()
        {
            foreach (var unitMovement in _creatures)
                unitMovement.SetDisableMaterial();

            if (_isAutomation)
                m_PlayerNpcController.ToMyTurn();
            else
                m_Environment.ChangeFaction();
                // StartCoroutine(WaitForChangeFaction());
        }

        private IEnumerator WaitForChangeFaction()
        {
            yield return new WaitForSeconds(0.5f);
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
            return _creatures.Find(x => Vector3.Distance(x.transform.position, unitPos) < 0.1f);
        }

        public void RemoveAgent(CreatureInGame unitMovement)
        {
            m_Environment.RemoveObject(unitMovement.gameObject, m_Faction);
            _creatures.Remove(unitMovement);

            if (_creatures.Count <= 0)
                GameFlowManager.Instance.OnGameOver.Invoke(3000);
        }

        public void ResetData()
        {
            _creatures = new List<CreatureInGame>();
        }

        #endregion
    }
}