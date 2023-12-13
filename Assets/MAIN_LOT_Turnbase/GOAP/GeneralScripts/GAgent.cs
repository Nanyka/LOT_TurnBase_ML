using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

namespace GOAP
{
    public class GAgent : MonoBehaviour
    {
        public List<GAction> Actions = new();
        public Dictionary<SubGoal, int> Goals = new();

        public GAction CurrentAction;
        public SubGoal CurrentGoal;

        public GInventory Inventory = new();
        public WorldStates Beliefs = new();

        protected GPlanner _planner;
        protected Queue<GAction> _actionQueue = new();

        public float RestInterval = 1f;
        private float _finishDistance = 3f;
        protected Transform _destination;
        public Vector3 _posDestination;
        protected IProcessUpdate m_ProcessUpdate;
        protected bool _isInvoke = false;

        protected virtual void Start()
        {
            GAction[] actions = GetComponents<GAction>();
            foreach (var action in actions)
                Actions.Add(action);
            
            // InvokeRepeating("APlusAlgorithm",1f,1f);
        }

        public virtual void APlusAlgorithm()
        {
            if (CurrentAction != null && CurrentAction.running)
            {
                float distanceToTarget = Vector3.Distance(_destination.position, transform.position);
                if (distanceToTarget <= _finishDistance)
                {
                    // Debug.Log("Distance to Goal: " + distanceToTarget);
                    if (!_isInvoke) 
                    {
                        Invoke("CompleteAction", CurrentAction.Duration);
                        _isInvoke = true;
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
                    if (CurrentAction.Target == null && CurrentAction.TargetTag != "")
                        CurrentAction.Target = GameObject.FindWithTag(CurrentAction.TargetTag);

                    if (CurrentAction.Target != null)
                    {
                        CurrentAction.running = true;

                        _destination.position = CurrentAction.Target.transform.position;
                        Transform
                            identifiedDestination =
                                CurrentAction.Target.transform
                                    .Find(
                                        "Destination"); // find if the target create a child called "Destination" or not. If so, the destination is this object rather than the parent Target
                        if (identifiedDestination != null)
                            _destination.position = identifiedDestination.position;

                        CurrentAction.mNavMeshAgent.SetDestination(_destination.position);
                    }
                }
                else
                {
                    _actionQueue = null;
                }
            }
        }

        // Wait to use when an action wanna finish from outside of APlusAlgorithm
        public virtual void FinishFromOutside() { }

        protected void CompleteAction()
        {
            CurrentAction.running = false;
            CurrentAction.PostPerform();
            _isInvoke = false;
        }

        public void SetIProcessUpdate(IProcessUpdate processUpdate)
        {
            CurrentAction.IsChasePosition = false;
            m_ProcessUpdate = processUpdate;
        }

        // Use for position target only
        public void SetIProcessUpdate(IProcessUpdate processUpdate, Vector3 positionTarget)
        {
            CurrentAction.IsChasePosition = true;
            _posDestination = positionTarget;
            m_ProcessUpdate = processUpdate;
        }
    }
}