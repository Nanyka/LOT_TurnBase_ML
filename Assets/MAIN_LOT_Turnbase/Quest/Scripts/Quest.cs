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
        // Vary in goal of the quest

        public EnvironmentData environmentData;
        public EntityType targetType;
        public Vector3 targetPos = Vector3.negativeInfinity;
        public GameMasterCondition winCondition;
        public List<Vector3> enableTiles;
        public int maxMovingTurn;
        public int maxTroop;
        public List<JIRemoteConfigManager.Reward> rewards;
    }
}
