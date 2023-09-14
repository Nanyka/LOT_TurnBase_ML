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
        public EnvironmentData environmentData;
        [Tooltip("Destroy this target and win the stage")]
        public EntityType targetType;
        public Vector3 targetPos = Vector3.negativeInfinity;
        [Tooltip("The protected object that will turn to fail if it is defeated")]
        public EntityType protectedType;
        public Vector3 protectedPos = Vector3.negativeInfinity;
        public GameMasterCondition winCondition;
        public List<Vector3> enableTiles;
        public int maxMovingTurn;
        public int maxTroop;
        public List<int> excellentRank = new(2);
        public List<JIRemoteConfigManager.Reward> rewards;
        public string tutorialForQuest;
    }
}
