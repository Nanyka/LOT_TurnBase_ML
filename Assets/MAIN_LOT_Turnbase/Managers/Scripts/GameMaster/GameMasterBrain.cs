using System.Linq;
using GOAP;
using JumpeeIsland;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    [RequireComponent(typeof(NPCGoalManager))]
    [RequireComponent(typeof(GameMasterSensor))]
    public class GameMasterBrain : GAgent
    {
        [SerializeField] private NPCGoalManager m_GoalManager;
        [SerializeField] private GameMasterSensor m_Sensor;
        private EnvironmentManager _environmentManager;
        private bool _isDone;

        #region APLUS ALGORITHM

        protected override void Start()
        {
            GameFlowManager.Instance.OnStartGame.AddListener(Init);
            AddActions();
            SetGoal();
        }

        private void Init(long arg0)
        {
            _environmentManager = GameFlowManager.Instance.GetEnvManager();
            _environmentManager.OnChangeFaction.AddListener(GameMasterDecision);
        }

        private void GameMasterDecision()
        {
            if (_environmentManager.GetCurrFaction() == FactionType.Player)
                return;

            m_Sensor.DetectEnvironment(Beliefs);
            APlusAlgorithm();
        }

        private void AddActions()
        {
            GAction[] actions = GetComponents<GAction>();
            foreach (var action in actions)
                Actions.Add(action);
        }

        protected override void APlusAlgorithm()
        {
            if (CurrentAction != null && CurrentAction.running)
            {
                if (_isDone)
                {
                    _isDone = false;
                    _actionQueue = null;
                    if (!isInvoke)
                    {
                        Invoke("CompleteAction", CurrentAction.Duration);
                        isInvoke = true;
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
    }
}