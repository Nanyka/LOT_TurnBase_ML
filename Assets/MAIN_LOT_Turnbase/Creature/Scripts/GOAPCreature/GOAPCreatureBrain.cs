using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    [RequireComponent(typeof(NPCGoalManager))]
    [RequireComponent(typeof(GOAPCreatureSensor))]
    public class GOAPCreatureBrain : GAgent
    {
        [SerializeField] private NPCGoalManager m_GoalManager;
        [SerializeField] private GOAPCreatureSensor m_Sensor;

        private GOAPCreatureInGame m_Creature;
        private bool _isDone;
        
        protected override void Start()
        {
            AddActions();
            SetGoal();
        }

        public void Init(GOAPCreatureInGame creatureInGame)
        {
            m_Creature = creatureInGame;
            m_Sensor.Init((CreatureData)m_Creature.GetEntityData());
        }

        #region APLUS ALGORITHM

        public void BrainInProcess()
        {
            m_Sensor.DetectEnvironment(Beliefs);
            APlusAlgorithm();
        }

        private void AddActions()
        {
            GAction[] actions = GetComponents<GAction>();
            foreach (var action in actions)
                Actions.Add(action);
        }

        public override void APlusAlgorithm()
        {
            if (CurrentAction != null && CurrentAction.running)
            {
                if (_isDone)
                {
                    _isDone = false;
                    _actionQueue = null;
                    if (!_isInvoke)
                    {
                        _isInvoke = true;
                        CompleteAction();
                    }
                }

                return;
            }

            if (_planner == null || _actionQueue == null)
            {
                _planner = new GPlanner();

                var sortedGoals = from entry in Goals orderby entry.Value descending select entry;

                foreach (var subGoal in sortedGoals)
                {
                    _actionQueue = _planner.Plan(Actions, subGoal.Key.DicSubGoal, Beliefs);
                    if (_actionQueue != null)
                    {
                        CurrentGoal = subGoal.Key;
                        break;
                    }
                }
            }

            if (_actionQueue != null && _actionQueue.Count == 0)
            {
                if (CurrentGoal.Remove)
                {
                    Goals.Remove(CurrentGoal);
                }

                _planner = null;
            }

            if (_actionQueue != null && _actionQueue.Count > 0)
            {
                CurrentAction = _actionQueue.Dequeue();
                if (CurrentAction.PrePerform())
                {
                    CurrentAction.running = true;
                    _isDone = true;
                    APlusAlgorithm();
                }
            }
        }

        #endregion

        #region GOAL MANAGER

        private void SetGoal()
        {
            var subGoals = m_GoalManager.GetCurrentGoal();
            foreach (var goal in subGoals)
            {
                Goals.Add(goal, goal.Weight);
            }
        }

        #endregion

        #region CONNECT TO CREATURE

        public void ResponseToCreature(int actionIndex)
        {
            m_Creature.ResponseAction(actionIndex);
        }

        public GOAPCreatureInGame GetCreature()
        {
            return m_Creature;
        }

        #endregion
    }
}
