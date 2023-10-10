using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class EnemyReplayController : MonoBehaviour, IFactionController
    {
        [SerializeField] private List<CreatureInGame> _creatures = new();
        private EnvironmentManager m_Environment;

        public void Init()
        {
            m_Environment = GameFlowManager.Instance.GetEnvManager();
        }

        public void AddCreatureToFaction(CreatureInGame creatureInGame)
        {
            _creatures.Add(creatureInGame);
        }

        public void MoveCreature(RecordAction action)
        {
            var currentCreature = _creatures.Find(t => Vector3.Distance(t.GetCurrentPosition(), action.AtPos) < 0.1f);
            if (currentCreature != null)
                currentCreature.MoveDirection(action.Action);
        }

        public MovementInspector GetMovementInspector()
        {
            return m_Environment.GetMovementInspector();
        }

        public void KickOffNewTurn()
        {
            throw new System.NotImplementedException();
        }

        public void WaitForCreature()
        {
            Debug.Log("Finish moving");
        }

        public FactionType GetFaction()
        {
            return FactionType.Enemy;
        }

        public EnvironmentManager GetEnvironment()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAgent(CreatureInGame creatureInGame)
        {
            throw new System.NotImplementedException();
        }

        public void ResetData()
        {
            throw new System.NotImplementedException();
        }
    }
}