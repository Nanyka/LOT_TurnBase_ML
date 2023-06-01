using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    [RequireComponent(typeof(NPCActionInferer))]
    public class EnemyFactionController : MonoBehaviour, IFactionController
    {
        [SerializeField] protected FactionType m_Faction = FactionType.Enemy;
        [SerializeField] protected List<NPCInGame> m_Enemies;
        [SerializeField] private Material _factionMaterial;
        [SerializeField] private Material _defaultMaterial;

        private EnvironmentManager m_Environment;
        private NPCActionInferer m_NpcActionInferer;
        private List<NPCInGame> _dummyNPCs = new();
        private int _skillCount;
        private int _responseCounter;
        
        public void AddCreatureToFaction(CreatureInGame creatureInGame)
        {
            m_Enemies.Add((NPCInGame) creatureInGame);
        }
        
        public void Init()
        {
            m_Environment = FindObjectOfType<EnvironmentManager>();
            m_Environment.OnChangeFaction.AddListener(ToMyTurn);
            m_Environment.OnOneTeamWin.AddListener(FinishRound);

            m_NpcActionInferer = GetComponent<NPCActionInferer>();
            
            InitiateNpcList();
        }

        private void InitiateNpcList()
        {
            SetTempIndex();
            m_NpcActionInferer.Init();
        }

        private void SetTempIndex()
        {
            for (int i = 0; i < m_Enemies.Count; i++)
            {
                var enemy = m_Enemies[i];
                enemy.InferMoving.AgentIndex = i;
            }
        }

        #region ONE TURN PIPELINE

        // Change unit colour from environmentManager when changing faction

        private void ToMyTurn()
        {
            if (m_Environment.GetCurrFaction() != m_Faction)
                return;

            // reset all agent's moving state
            foreach (var enemy in m_Enemies)
                enemy.ResetMoveState(_factionMaterial);

            KickOffNewTurn();
        }

        // Select available agents and kick off the first inference after without-brain inference
        public void KickOffNewTurn()
        {
            // Just select jumpers who still not move this turn
            _dummyNPCs.Clear();
            foreach (var enemy in m_Enemies)
                if (enemy.CheckUsedThisTurn() == false)
                    _dummyNPCs.Add(enemy);

            if (_dummyNPCs.Count > 0)
            {
                _skillCount = 0;
                m_NpcActionInferer.ResetSkillCache();
                SelfInferenceBrainStorming(_dummyNPCs);
                StartInferAgentsAction(_dummyNPCs);
            }
            else
                EndTurn();
        }

        // Do inference without brain, just ask for all direction and collect relevant rewards
        private void SelfInferenceBrainStorming(IEnumerable<NPCInGame> jumperForGames)
        {
            var jumpers = jumperForGames as NPCInGame[] ?? jumperForGames.ToArray();
            foreach (var jumper in jumpers)
            {
                for (int i = 0; i <= 4; i++)
                {
                    DummyAction action = new DummyAction(jumper.RespondFromAction(i));
                    m_NpcActionInferer.AddActionToCache(action);
                }
            }
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
                m_NpcActionInferer.AddActionToCache(jumperForGame.InferMoving);

            // Move to next skill
            _skillCount++;
            StartInferAgentsAction(_dummyNPCs);
        }

        #endregion

        #region END TURN

        protected void EndTurn()
        {
            var attackAmount = 0;
            // Attack nearby enemy
            foreach (var enemy in m_Enemies.Where(enemy => enemy.GetJumpStep() != 0))
            {
                attackAmount++;
                enemy.Attack();
            }

            // call for the end-turn event
            StartCoroutine(WaitForChangeFaction(attackAmount * 1f));
        }

        private IEnumerator WaitForChangeFaction(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            // Set all npc to default color to show it disable state
            foreach (var enemy in m_Enemies)
                enemy.SetMaterial(_defaultMaterial);

            m_Environment.ChangeFaction();
            // m_Environment.OnChangeFaction.Invoke();
        }

        protected void FinishRound(FactionType faction)
        {
            Debug.Log("NPC end game");
        }

        #endregion

        #region GET & SET

        public MovementInspector GetMovementInspector()
        {
            return m_Environment.GetMovementInspector();
        }

        // Remove dieJumper from environment
        public void RemoveAgent(NPCInGame jumper)
        {
            m_Environment.RemoveObject(jumper.gameObject, m_Faction);
            m_Enemies.Remove(jumper);
            SetTempIndex();
        }

        public List<NPCInGame> GetEnemies()
        {
            return m_Enemies;
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
            m_Environment.RemoveObject(creatureInGame.gameObject,m_Faction);
            m_Enemies.Remove((NPCInGame) creatureInGame);
            SetTempIndex();
        }

        public void ResetData()
        {
            m_Enemies = new List<NPCInGame>();
            _dummyNPCs = new List<NPCInGame>();
        }

        #endregion
    }
}