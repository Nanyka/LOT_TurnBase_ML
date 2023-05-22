using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LOT_Turnbase
{
    public class PlayerFactionController : MonoBehaviour
    {
        [SerializeField] private FactionType m_Faction = 0;
        [SerializeField] private Material _factionMaterial;
        [SerializeField] private List<CreatureInGame> _creatures = new(4);
        [SerializeField] private Material _defaultMaterial;
        
        private EnvironmentManager m_Environment;
        private Camera _camera;
        private int _layerMask = 1 << 3;
        private CreatureInGame _currentUnit;
        private int _countMovedUnit;

        private void Start()
        {
            m_Environment = FindObjectOfType<EnvironmentManager>();
            m_Environment.OnChangeFaction.AddListener(ToMyTurn);
            m_Environment.OnTouchSelection.AddListener(MoveToward);
            // m_Environment.OnOneTeamWin.AddListener(EndGame);
            // m_Environment.OnReset.AddListener(ResetGame);
            MainUI.Instance.OnClickIdleButton.AddListener(SetCurrentUnitIdle);
            
            _camera = Camera.main;
        }

        // Add creature to this faction manager from it's Init()
        public void AddCreatureToFaction(CreatureInGame creature)
        {
            _creatures.Add(creature);
        }

        public void FactionSetUp(List<CreatureInGame> creatureInGames)
        {
            _creatures = creatureInGames;
            _defaultMaterial = _creatures[0].GetMaterial();
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

            foreach (var unitMovement in _creatures)
                unitMovement.NewTurnReset(_factionMaterial);

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

            MainUI.Instance.OnShowCreatureInfo.Invoke(getUnitAtPos);
        }

        private void MoveToward(int direction)
        {
            if (m_Environment.GetCurrFaction() != m_Faction)
                return;
            
            //TODO Control player unit
            Debug.Log(_currentUnit);

            // Move unit along dedicated direction
            _currentUnit.MoveDirection(direction);
        }

        public void UnitMoved()
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
            _currentUnit.SetUsedState();
            UnitMoved();
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
            m_Environment.OnChangeFaction.Invoke();
        }

        // private void EndGame(int winFaction)
        // {
        //     Debug.Log($"End game from player faction");
        //     // UIManager.Instance.OnGameOver.Invoke(winFaction);
        //     // StartCoroutine(WaitToReset());
        // }
        //
        // private IEnumerator WaitToReset()
        // {
        //     yield return new WaitForSeconds(3f);
        //     ResetGame();
        // }

        // private void ResetGame()
        // {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // }

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

        // public Collider GetPlatformCollider()
        // {
        //     return m_Environment.GetPlatformCollider();
        // }

        public MovementInspector GetMovementCalculator()
        {
            return m_Environment.GetMovementCalculator();
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

        #endregion
    }
}