using System.Collections.Generic;
using GOAP;
using UnityEngine;

namespace GOAP
{
    public class EnemyGoalManager : MonoBehaviour
    {
        [SerializeField] private List<SubGoal> _mCurrentGoal = new();
    
        public List<SubGoal> GetCurrentGoal()
        {
            return _mCurrentGoal;
        }
    }
}

