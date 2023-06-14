using System.Collections.Generic;
using WebSocketSharp;

namespace GOAP
{
    [System.Serializable]
    public class SubGoal
    {
        public Dictionary<string, int> DicSubGoal;
        public string GoalId;
        public bool Remove;
        public int Weight;

        public SubGoal()
        {
            DicSubGoal = new Dictionary<string, int>();
            if (GoalId.IsNullOrEmpty())
                return;
            DicSubGoal.Add(GoalId, Weight);
        }

        public SubGoal(string s, int i, bool r)
        {
            DicSubGoal = new Dictionary<string, int>();
            DicSubGoal.Add(s, i);
            GoalId = s;
            Weight = i;
            Remove = r;
        }
    }
}