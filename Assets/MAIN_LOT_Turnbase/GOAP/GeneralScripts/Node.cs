using System.Collections.Generic;

namespace GOAP
{
    public class Node
    {
        public Node Parent;
        public float Cost;
        public Dictionary<string, int> State;
        public GAction Action;

        public Node()
        {
            
        }
        
        public Node(Node parent, float cost, Dictionary<string, int> worldState, GAction action)
        {
            Parent = parent;
            Cost = cost;
            State = new Dictionary<string, int>(worldState);
            Action = action;
        }

        public Node(Node parent, float cost, Dictionary<string, int> worldState, Dictionary<string, int> beliefState,
            GAction action)
        {
            Parent = parent;
            Cost = cost;
            State = new Dictionary<string, int>(worldState);
            foreach (var state in beliefState)
                if (!State.ContainsKey(state.Key))
                    State.Add(state.Key, state.Value);

            Action = action;
        }
    }
}