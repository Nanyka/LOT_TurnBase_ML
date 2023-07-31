using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "GameMasterDecision", menuName = "JumpeeIsland/GameMasterDecision", order = 5)]
    public class GameMasterDecision : ScriptableObject
    {
        [SerializeField] private GameMasterCondition Condition;
        [SerializeField] private string State;
        [SerializeField] private int GapDuration;

        private int currentGapCount;

        public string MakeDecision()
        {
            if (currentGapCount > 0)
            {
                currentGapCount--;
                return String.Empty;
            }
            
            if (Condition.CheckPass() == false)
                return String.Empty;
            
            currentGapCount = GapDuration;
            return State;
        }
    }
}