using System.Collections.Generic;

namespace GOAP
{
    public class GPlanner
    {
        public Queue<GAction> Plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates beliefStates)
        {
            List<GAction> usableActions = new List<GAction>();
            foreach (var action in actions)
            {
                if (action.IsAchievable())
                    usableActions.Add(action);
            }

            List<Node> leaves = new List<Node>();

            Node start = new Node(null, 0, GWorld.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);

            bool success = BuildGraph(start, leaves, usableActions, goal);

            if (!success)
            {
                // Debug.Log("NO PLAN");
                return null;
            }

            Node cheapest = null;
            foreach (var leaf in leaves)
            {
                if (cheapest == null)
                    cheapest = leaf;
                else
                {
                    if (leaf.Cost < cheapest.Cost)
                        cheapest = leaf;
                }
            }

            List<GAction> result = new List<GAction>();
            Node n = cheapest;
            while (n != null)
            {
                if (n.Action != null)
                {
                    result.Insert(0,n.Action);
                }
                n = n.Parent;
            }

            Queue<GAction> queue = new Queue<GAction>();
            foreach (var action in result)
            {
                queue.Enqueue(action);
            }
            
            // Debug.Log("The plan is: ");
            // foreach (var action in queue)
            // {
            //     Debug.Log("Q: "+action.ActionName);
            // }

            return queue;
        }

        private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal)
        {
            bool foundPath = false;

            foreach (var action in usableActions)
            {
                if (action.IsAchievableGiven(parent.State))
                {
                    Dictionary<string, int> currentState = new Dictionary<string, int>(parent.State);
                    foreach (var effect in action.DicAfterEffects)
                    {
                        if (!currentState.ContainsKey(effect.Key))
                        {
                            currentState.Add(effect.Key,effect.Value);
                        }
                    }

                    Node node = new Node(parent, parent.Cost + action.Cost, currentState, action);

                    if (GoalAchieved(goal, currentState))
                    {
                        leaves.Add(node);
                        foundPath = true;
                    }
                    else
                    {
                        List<GAction> subSet = ActionSubSet(usableActions, action);
                        bool found = BuildGraph(node, leaves, subSet, goal);

                        if (found)
                            foundPath = true;
                    }
                }
            }

            return foundPath;
        }

        private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
        {
            foreach (var g in goal)
            {
                if (!state.ContainsKey(g.Key))
                {
                    return false;
                }
            }

            return true;
        }

        private List<GAction> ActionSubSet(List<GAction> usableActions, GAction removeAction)
        {
            List<GAction> subSet = new List<GAction>();
            foreach (var action in usableActions)
            {
                if (!action.Equals(removeAction))
                    subSet.Add(action);
            }

            return subSet;
        }
    }
}