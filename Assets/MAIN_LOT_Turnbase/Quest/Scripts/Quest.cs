using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "Quest", menuName = "JumpeeIsland/Quest", order = 4)]
    public class Quest : ScriptableObject
    {
        // Limit tile
        // Limit turn to move
        // Limit amount of troops
        // Various goals of the quest
        // Provide tutorial if needed

        public bool isFinalBoss;
        public string questMessage;
        public EnvironmentData environmentData;
        public GameMasterCondition winCondition;
        public List<Vector3> enableTiles;
        public List<EndGameUnit> EndGameUnits;
        public int maxMovingTurn;
        public int maxTroop;
        public List<int> excellentRank = new(2);
        public List<JIRemoteConfigManager.Reward> rewards;
        public string tutorialForQuest;
    }

    [Serializable]
    public class EndGameUnit
    {
        public EntityType targetType;
        public Vector3 targetPos;
    }
}
