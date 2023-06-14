using System;
using System.Collections.Generic;
using GOAP;
using JumpeeIsland;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class GameMasterSensor : MonoBehaviour
    {
        [SerializeField] private List<GameMasterDecision> _decisions;
    
        public void DetectEnvironment(WorldStates beliefs)
        {
            foreach (var decision in _decisions)
            {
                var responseDecision = decision.MakeDecision();
                if (String.IsNullOrEmpty(responseDecision))
                    continue;
                beliefs.ModifyState(responseDecision,0);
            }
        }
    }
}
