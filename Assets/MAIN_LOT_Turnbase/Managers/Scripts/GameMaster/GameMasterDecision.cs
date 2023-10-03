using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
        [SerializeField] private List<SpawnObjectDecision> _spawnDecisions;
        // [SerializeField] private int _gapDuration;
        [SerializeField] private int _maxSpawningAmount;

        public string GetObjectToSpawn()
        {
            if (_mainCondition.CheckPass() == false)
                return null;

            var adjustProportion = new List<int>(_spawnDecisions.Count);
            var cacheIncrement = 0;
            foreach (var t in _spawnDecisions)
            {
                adjustProportion.Add(cacheIncrement + t.Proportion);
                cacheIncrement += t.Proportion;
            }

            var randomNumber = Random.Range(0, _spawnDecisions.Sum(t => t.Proportion));
            for (int j = 0; j < _spawnDecisions.Count; j++)
            {
                if (randomNumber < adjustProportion[j])
                    return _spawnDecisions[j].ObjectName;
            }

            return _spawnDecisions.OrderByDescending(t => t.Proportion).FirstOrDefault()?.ObjectName;
        }

        public bool CheckGetThrough()
        {
            return _getThroughCondition.CheckPass();
        }

        public EntityType GetEntityType()
        {
            return m_EntityType;
        }

        public int GetMaxSpawningAmount()
        {
            return _maxSpawningAmount;
        }
    }

    [Serializable]
    public class SpawnObjectDecision
    {
        public string ObjectName;
        public int Proportion;
    }
}