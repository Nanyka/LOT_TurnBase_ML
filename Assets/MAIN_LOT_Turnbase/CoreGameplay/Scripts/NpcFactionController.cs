using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    [RequireComponent(typeof(NPCActionInferer))]
    public class NpcFactionController : MonoBehaviour, IFactionController
    {
        [SerializeField] protected FactionType m_Faction = FactionType.Enemy;

        protected List<NPCInGame> m_NpcUnits = new();
        protected EnvironmentManager m_Environment;
        protected NPCActionInferer m_NpcActionInferer;
        private List<NPCInGame> _dummyNPCs = new();
        private int _skillCount;
        private int _responseCounter;
        protected Camera _camera;
        private int _layerMask = 1 << 7;

        public void AddCreatureToFaction(CreatureInGame creatureInGame)
        {
            m_NpcUnits.Add((NPCInGame)creatureInGame);
        }

        public virtual void Init()
        {
            m_Environment = GameFlowManager.Instance.GetEnvManager();
            m_Environment.OnChangeFaction.AddListener(ToMyTurn);
            m_NpcActionInferer = GetComponent<NPCActionInferer>();
            m_NpcActionInferer.Init();
            _camera = Camera.main;
        }

        private void SetTempIndex()
        {
            for (int i = 0; i < _dummyNPCs.Count; i++)
            {
                var npcUnit = _dummyNPCs[i];
                npcUnit.InferMoving.AgentIndex = i;
                npcUnit.InferMoving.JumpCount = 0;
                npcUnit.InferMoving.VoteAmount = 0;
            }
        }

        #region ONE TURN PIPELINE

        public virtual void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (MainUI.Instance.IsInteractable == false || PointingChecker.IsPointerOverUIObject())
                    return;

                var moveRay = _camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(moveRay, out var moveHit, 100f, _layerMask))
                    return;

                var getUnitAtPos = m_NpcUnits.Find(x => Vector3.Distance(x.transform.position, moveHit.transform.position) < 0.1f);
                if (getUnitAtPos != null)
                    MainUI.Instance.OnShowInfo.Invoke(getUnitAtPos);
            }
        }

        // Change unit colour from environmentManager when changing faction

        public virtual void ToMyTurn()
        {
            if (m_Environment.GetCurrFaction() != m_Faction)
                return;

            // reset all agent's moving state
            foreach (var npcUnit in m_NpcUnits)
                npcUnit.NewTurnReset();

            KickOffNewTurn();
        }

        // Select available agents and kick off the first inference after without-brain inference
        public void KickOffNewTurn()
        {
            // Just select jumpers who still not move this turn
            _dummyNPCs.Clear();
            foreach (var npcUnit in m_NpcUnits)
                if (npcUnit.CheckUsedThisTurn() == false)
                    _dummyNPCs.Add(npcUnit);

            if (_dummyNPCs.Count > 0)
            {
                _skillCount = 0;
                SetTempIndex();
                m_NpcActionInferer.ResetSkillCache();
                m_NpcActionInferer.GatherSkillFromJumpers();
                SelfInferenceBrainStorming(_dummyNPCs);
                StartInferAgentsAction(_dummyNPCs);
            }
            else
                EndTurn();
        }

        // Do inference without brain, just ask for all direction and collect relevant rewards
        private void SelfInferenceBrainStorming(IEnumerable<NPCInGame> npcJumpers)
        {
            var jumpers = npcJumpers as NPCInGame[] ?? npcJumpers.ToArray();
            foreach (var jumper in jumpers)
                jumper.SelfInfer(m_NpcActionInferer);
        }

        private void StartInferAgentsAction(IEnumerable<NPCInGame> jumperForGames)
        {
            if (_skillCount < m_NpcActionInferer.GetSkillAmount() &&
                m_NpcActionInferer.GetSkillByIndex(_skillCount) != null)
            {
                var jumpers = jumperForGames as NPCInGame[] ?? jumperForGames.ToArray();
                // reset counter before collect action
                _responseCounter = 0;
                // Ask for action based on skill count if still not over all skill
                foreach (var jumper in jumpers)
                {
                    // Infer action & add to jumper cache as currentAction when other idle
                    if (jumper._isSwitchBrain)
                        jumper.SetBrain(m_NpcActionInferer.GetSkillByIndex(_skillCount).GetModel());
                    jumper.AskForAction();
                }
            }
            else
            {
                m_NpcActionInferer.ActionBrainstorming();
            }
        }

        public void WaitForCreature()
        {
            _responseCounter++;

            // if all agent response, add action to cache at skillManager
            if (_responseCounter != _dummyNPCs.Count) return;

            foreach (var jumperForGame in _dummyNPCs)
                if (jumperForGame._isSwitchBrain)
                    m_NpcActionInferer.AddActionToCache(jumperForGame.InferMoving);

            // Move to next skill
            _skillCount++;
            StartInferAgentsAction(_dummyNPCs);
        }

        #endregion

        #region END TURN

        private async void EndTurn()
        {
            // Set all npc to default color to show it disable state
            // foreach (var npcUnit in m_NpcUnits)
            //     npcUnit.SetDisableMaterial();
            //
            // m_Environment.ChangeFaction();

            await WaitForChangeFaction(500);
        }

        private async Task WaitForChangeFaction(int miliseconds)
        {
            await Task.Delay(miliseconds);

            // Set all npc to default color to show it disable state
            foreach (var npcUnit in m_NpcUnits)
                npcUnit.SetDisableMaterial();

            m_Environment.ChangeFaction();
            // m_Environment.OnChangeFaction.Invoke();
        }

        #endregion

        #region GET & SET

        public MovementInspector GetMovementInspector()
        {
            return m_Environment.GetMovementInspector();
        }

        public List<NPCInGame> GetEnemies()
        {
            return _dummyNPCs;
        }

        public FactionType GetFaction()
        {
            return m_Faction;
        }

        public EnvironmentManager GetEnvironment()
        {
            return m_Environment;
        }

        public void RemoveAgent(CreatureInGame creatureInGame)
        {
            Debug.Log("Require some visual stuff here");
            m_Environment.RemoveObject(creatureInGame.gameObject, m_Faction);
            m_NpcUnits.Remove((NPCInGame)creatureInGame);
            SetTempIndex();
        }

        public void ResetData()
        {
            m_NpcUnits = new List<NPCInGame>();
            _dummyNPCs = new List<NPCInGame>();
        }

        #endregion
    }
}