using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class GAgent : MonoBehaviour
    {
        public List<GAction> Actions = new List<GAction>();
        public Dictionary<SubGoal, int> Goals = new Dictionary<SubGoal, int>();

        public GAction CurrentAction;
        public SubGoal CurrentGoal;

        public GInventory Inventory = new GInventory();
        public WorldStates Beliefs = new WorldStates();

        protected GPlanner _planner;
        protected Queue<GAction> _actionQueue;

        [SerializeField] private float finishDistance = 3f;
        protected Vector3 destination = Vector3.zero;
        protected bool isInvoke = false;

        protected virtual void Start()
        {
            GAction[] actions = GetComponents<GAction>();
            foreach (var action in actions)
                Actions.Add(action);
            
            InvokeRepeating("APlusAlgorithm",1f,1f);
        }

        // private void LateUpdate()
        // {
        //     APlusAlgorithm();
        // }

        protected virtual void APlusAlgorithm()
        {
            if (CurrentAction != null && CurrentAction.running)
            {
                float distanceToTarget = Vector3.Distance(destination, transform.position);
                if (distanceToTarget <= finishDistance)
                {
                    // Debug.Log("Distance to Goal: " + distanceToTarget);
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
                    if (CurrentAction.Target == null && CurrentAction.TargetTag != "")
                        CurrentAction.Target = GameObject.FindWithTag(CurrentAction.TargetTag);

                    if (CurrentAction.Target != null)
                    {
                        CurrentAction.running = true;

                        destination = CurrentAction.Target.transform.position;
                        Transform
                            identifiedDestination =
                                CurrentAction.Target.transform
                                    .Find(
                                        "Destination"); // find if the target create a child called "Destination" or not. If so, the destination is this object rather than the parent Target
                        if (identifiedDestination != null)
                            destination = identifiedDestination.position;

                        CurrentAction.mNavMeshAgent.SetDestination(destination);
                    }
                }
                else
                {
                    _actionQueue = null;
                }
            }
        }

        protected void CompleteAction()
        {
            CurrentAction.running = false;
            CurrentAction.PostPerform();
            isInvoke = false;
        }
    }
}