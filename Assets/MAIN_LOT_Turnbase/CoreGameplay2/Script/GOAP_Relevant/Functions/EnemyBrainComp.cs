using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP
{
    public class EnemyBrainComp : GAgent
    {
        [SerializeField] private Transform _mainTranform;
        [SerializeField] private EnemyGoalManager m_GoalManager;
        [SerializeField] private float _stopDistance = 5;

        private SubGoal _currentGoal;
        private bool _isActive;

        #region INITIATE

        protected virtual void OnEnable()
        {
            _isActive = true;

            SetActions();
            SetGoal();
            APlusAlgorithm();
        }

        protected void OnDisable()
        {
            _isActive = false;
        }

        protected override void Start() { }

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

        protected void DetectTarget(GameObject target)
        {
            Beliefs.ModifyState("FoundPlayer", 1);
            Inventory.AddItem(target);
        }

        #endregion

        #region A* ALGORITHM

        public override void APlusAlgorithm()
        {
            if (_isActive == false)
                return;
            
            // Debug.Log("AStar algorithm");
            if (CurrentAction != null && CurrentAction.running)
            {
                if (CurrentAction.IsAutoComplete)
                {
                    if (!_isInvoke)
                    {
                        WaitForPostPerformance();
                        _isInvoke = true;
                        // Invoke("CompleteAction", CurrentAction.Duration);
                    }
                }
                // else
                // {
                //     float distanceToTarget = 0f;
                //     if (CurrentAction.IsChasePosition)
                //         distanceToTarget = Vector3.Distance(_posDestination, transform.position);
                //     else
                //         distanceToTarget = Vector3.Distance(_destination.position, transform.position);
                //
                //     if (distanceToTarget < _stopDistance)
                //         WhenChaseTarget();
                //     else
                //         WaitForPostPerformance();
                // }

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
                    if (CurrentAction.Target != null)
                    {
                        CurrentAction.running = true;
                        _destination = CurrentAction.Target.transform;
                    }
                    else if (CurrentAction.IsAutoComplete || CurrentAction.IsChasePosition)
                    {
                        CurrentAction.running = true;
                    }
                    else
                    {
                        WaitForPostPerformance();
                        return;
                    }

                    Invoke(nameof(APlusAlgorithm), CurrentAction.Duration);
                }
                else
                {
                    _actionQueue.Clear();
                }
            }
            else
                WhenNoSelectedAction();
        }
        
        protected override void WhenChaseTarget()
        {
            if (m_ProcessUpdate != null)
            {
                // TODO: Use a separate script to move object and return APlusAlgorithm when get destination
                m_ProcessUpdate.MoveToDestination(_mainTranform,
                    CurrentAction.IsChasePosition ? _posDestination : CurrentAction.Target.transform.position);
                // Invoke(nameof(APlusAlgorithm), RestInterval);
            }
            else
            {
                Debug.Log("Null processUpdate");
                WaitForPostPerformance();
            }
        }

        private void WhenNoSelectedAction()
        {
            Beliefs.ModifyState("Idle", 0);
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
            await Task.Delay(Mathf.RoundToInt(CurrentAction.Duration)*1000);
            
            if (CurrentAction.IsWaitAndStop)
                m_ProcessUpdate?.StopProcess();
            
            CurrentAction.running = false;
            CurrentAction.PostPerform();
            m_ProcessUpdate = null;
            _isInvoke = false;
            
            APlusAlgorithm();
        }

        #endregion
    }
}