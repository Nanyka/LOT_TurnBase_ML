using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class EnemyBrainComp : GAgent, IBrain
    {
        [SerializeField] private Transform _mainTranform;
        [SerializeField] private EnemyGoalManager m_GoalManager;

        private SubGoal _currentGoal;
        private bool _isActive;

        #region INITIATE

        protected virtual void OnEnable()
        {
            _isActive = true;

            SetActions();
            SetGoal();
            // APlusAlgorithm();
            // TODO: After restructure GWorld as the fist loading, call APlusAlgorithm directly instead of Invoke
            Invoke(nameof(APlusAlgorithm), 1f);
        }

        protected void OnDisable()
        {
            CancelInvoke();
            _isActive = false;
        }

        protected override void Start()
        {
        }

        private void SetActions()
        {
            Actions.Clear();
            GAction[] actions = GetComponents<GAction>();
            foreach (var action in actions)
                Actions.Add(action);
        }

        public void SetBrainDisable()
        {
            Beliefs.ClearStates();
            _isActive = false;
        }

        #endregion

        #region GOAL MANAGER

        protected void SetGoal()
        {
            Goals.Clear();
            var subGoals = m_GoalManager.GetCurrentGoal();
            foreach (var goal in subGoals)
            {
                Goals.Add(new SubGoal(goal.GoalId, goal.Weight, goal.Remove), goal.Weight);
            }
        }

        #endregion

        #region SENSORs

        public void AddBeliefs(string state)
        {
            Beliefs.ModifyState(state, 1);
        }

        #endregion

        #region A* ALGORITHM

        public override void APlusAlgorithm()
        {
            if (_isActive == false)
                return;

            if (CurrentAction != null && CurrentAction.running)
                return;

            if (_planner == null || _actionQueue == null)
            {
                _planner = new GPlanner();

                var sortedGoals = from entry in Goals orderby entry.Value descending select entry;

                foreach (var subGoal in sortedGoals)
                {
                    _actionQueue = _planner.Plan(Actions, subGoal.Key.DicSubGoal, Beliefs);
                    if (_actionQueue != null)
                    {
                        _currentGoal = subGoal.Key;
                        break;
                    }
                }
            }

            if (_actionQueue != null && _actionQueue.Count == 0)
            {
                if (_currentGoal.Remove)
                    Goals.Remove(_currentGoal);

                _planner = null;
            }

            if (_actionQueue != null && _actionQueue.Count > 0)
            {
                CurrentAction = _actionQueue.Dequeue();
                if (CurrentAction.PrePerform())
                {
                    if (CurrentAction.Target != null || CurrentAction.IsChasePosition)
                    {
                        CurrentAction.running = true;
                        WhenChaseTarget();
                        // _destination = CurrentAction.Target.transform;
                    }
                    else
                    {
                        if (CurrentAction.IsAutoComplete)
                        {
                            CurrentAction.running = true;

                            if (!_isInvoke)
                            {
                                WaitForPostPerformance();
                                _isInvoke = true;
                            }
                        }
                        else
                            WaitForPostPerformance();
                    }
                }
                else
                {
                    _actionQueue.Clear();
                    APlusAlgorithm();
                }
            }
            else
                WhenNoSelectedAction();
        }

        private void WhenChaseTarget()
        {
            if (m_ProcessUpdate != null)
            {
                m_ProcessUpdate.StartProcess(_mainTranform,
                    CurrentAction.IsChasePosition ? _posDestination : CurrentAction.Target.transform.position);
            }
            else
            {
                Debug.Log("Null processUpdate");
                WaitForPostPerformance();
            }
        }

        private void WhenNoSelectedAction()
        {
            Beliefs.ModifyState("Idle", 1);
            // Debug.Log("APlus from NoSelectedAction");
            Invoke(nameof(APlusAlgorithm), RestInterval);
        }

        private async void WaitForPostPerformance()
        {
            if (CurrentAction.IsWaitAndStop == false)
                m_ProcessUpdate?.StopProcess();

            await CompleteAction();
        }

        public override async void FinishFromOutside()
        {
            await CompleteAction();
        }

        private new async Task CompleteAction()
        {
            if (CurrentAction == null)
                return;
            
            await Task.Delay(Mathf.RoundToInt(CurrentAction.Duration) * 1000);

            if (CurrentAction.IsWaitAndStop)
                m_ProcessUpdate?.StopProcess();

            CurrentAction.running = false;
            CurrentAction.PostPerform();
            m_ProcessUpdate = null;
            _isInvoke = false;

            // Debug.Log("APlus from CompleteAction");
            APlusAlgorithm();
        }

        #endregion

        public GInventory GetInventory()
        {
            return Inventory;
        }

        public WorldStates GetBeliefStates()
        {
            return Beliefs;
        }

        public void RefreshBrain()
        {
            _isActive = true;
            ResetBrain();
            APlusAlgorithm();
        }

        public void ResetBrain()
        {
            Beliefs.ClearStates();
            Inventory.ClearInventory();
            CurrentAction = null;
            // APlusAlgorithm();
        }
    }

    public interface IBrain
    {
        public GInventory GetInventory();
        public WorldStates GetBeliefStates();
        public void RefreshBrain();
    }
}