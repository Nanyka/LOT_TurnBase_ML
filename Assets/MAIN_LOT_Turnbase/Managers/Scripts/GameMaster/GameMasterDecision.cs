using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "GameMasterDecision", menuName = "TurnBase/GameMasterDecision", order = 5)]
    public class GameMasterDecision : ScriptableObject
    {
        [SerializeField] private List<GameMasterCondition> Conditions;
        [SerializeField] private string State;

        public string MakeDecision()
        {
            foreach (var condition in Conditions)
            {
                if (condition.PassCondition() == false)
                    return String.Empty;
            }

            return State;
        }
    }
}