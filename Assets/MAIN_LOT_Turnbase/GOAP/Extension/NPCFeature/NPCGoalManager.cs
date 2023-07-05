using System.Collections.Generic;
using GOAP;
using UnityEngine;

public class NPCGoalManager : MonoBehaviour
{
    [SerializeField] private List<SubGoal> _mCurrentGoal = new();
    
    public List<SubGoal> GetCurrentGoal()
    {
        return _mCurrentGoal;
    }
}
