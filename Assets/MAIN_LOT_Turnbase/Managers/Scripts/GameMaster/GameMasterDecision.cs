using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "GameMasterDecision", menuName = "JumpeeIsland/GameMasterDecision", order = 5)]
    public class GameMasterDecision : ScriptableObject
    {
        [SerializeField] private EntityType m_EntityType;
        [Tooltip("If it true, don't make this decision")]
        [SerializeField] private GameMasterCondition _getThroughCondition;
        [Tooltip("If it false, don't make this decision")]
        [SerializeField] private GameMasterCondition _mainCondition;
        [SerializeField] private string[] _objects;
        [SerializeField] private int _gapDuration;

        [SerializeField] private int currentGapCount;

        public IEnumerable<string> GetObjectsToSpawn()
        {
            if (currentGapCount > 0)
            {
                currentGapCount--;
                return null;
            }
            
            if (_mainCondition.CheckPass() == false)
                return null;
            
            currentGapCount = _gapDuration;
            return _objects;
        }

        public bool CheckGetThrough()
        {
            return _getThroughCondition.CheckPass();
        }

        public EntityType GetEntityType()
        {
            return m_EntityType;
        }
    }
}