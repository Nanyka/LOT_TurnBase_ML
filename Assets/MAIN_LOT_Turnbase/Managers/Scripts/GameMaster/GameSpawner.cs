using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using WebSocketSharp;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    [Serializable]
    public class SpawningTier
    {
        public GameMasterCondition tierCondition;
        public GameMasterDecision spawnGameDecision;
        public GameMasterDecision spawnTreeDecision;
        public GameMasterDecision spawnCollectableObjDecision;
    }

    public class GameSpawner : MonoBehaviour
    {
        [SerializeField] private List<SpawningTier> _spawningTiers;

        private EnvironmentManager _environmentManager;

        private void Start()
        {
            GameFlowManager.Instance.OnStartGame.AddListener(Init);
        }

        private void Init(long arg0)
        {
            _environmentManager = GameFlowManager.Instance.GetEnvManager();
            _environmentManager.OnChangeFaction.AddListener(MakeSpawningDecision);
        }

        private void MakeSpawningDecision()
        {
            if (_environmentManager.GetCurrFaction() == FactionType.Player)
                return;

            // withdrawn the SpawningTier that fit with player's condition
            if (_spawningTiers.Any() == false)
                return;

            SpawningTier currentTier = _spawningTiers[0];
            foreach (var tier in _spawningTiers)
            {
                if (tier.tierCondition.CheckPass() == false)
                    break;
                currentTier = tier;
            }

            // Select game to spawn if spawning condition is passed
            if (currentTier.spawnGameDecision != null)
            {
                currentTier.spawnGameDecision.CheckGetThrough();
                if (currentTier.spawnGameDecision.CheckGetThrough() == false)
                {
                    var spawnGame = currentTier.spawnGameDecision.GetObjectToSpawn();
                    if (spawnGame.IsNullOrWhitespace() == false)
                    {
                        var creatureAmount = SavingSystemManager.Instance.GetEnvironmentData().EnemyData
                            .Count(t => t.CreatureType == CreatureType.MOVER);
                        var spawningAmount = Random.Range(0,
                            currentTier.spawnGameDecision.GetMaxSpawningAmount() - creatureAmount);
                        for (int i = 0; i < spawningAmount; i++)
                        {
                            var availableTile = GameFlowManager.Instance.GetEnvManager()
                                .GetRandomAvailableTile();
                            if (availableTile.x.CompareTo(float.NegativeInfinity) == 1)
                            {
                                SavingSystemManager.Instance.OnSpawnMovableEntity(spawnGame, availableTile);
                            }
                        }
                    }
                }
            }

            // Select resources to spawn if spawning condition is passed
            if (currentTier.spawnTreeDecision != null)
            {
                if (currentTier.spawnTreeDecision.CheckGetThrough() == false)
                {
                    var spawnTree = currentTier.spawnTreeDecision.GetObjectToSpawn();
                    if (spawnTree.IsNullOrWhitespace() == false)
                    {
                        var creatureAmount = SavingSystemManager.Instance.GetEnvironmentData().EnemyData
                            .Count(t => t.CreatureType == CreatureType.MOVER);
                        var spawningAmount = Random.Range(0,
                            currentTier.spawnGameDecision.GetMaxSpawningAmount() - creatureAmount);
                        for (int i = 0; i < spawningAmount; i++)
                        {
                            var availableTile = GameFlowManager.Instance.GetEnvManager()
                                .GetRandomAvailableTile();
                            if (availableTile.x.CompareTo(float.NegativeInfinity) == 1)
                            {
                                SavingSystemManager.Instance.OnSpawnMovableEntity(spawnTree, availableTile);
                            }
                        }
                    }
                }
            }

            // Select collectable objects to spawn if spawning condition is passed
            if (currentTier.spawnCollectableObjDecision != null)
            {
                if (currentTier.spawnCollectableObjDecision.CheckGetThrough() == false)
                {
                    var spawnTree = currentTier.spawnCollectableObjDecision.GetObjectToSpawn();
                    if (spawnTree.IsNullOrWhitespace() == false)
                    {
                        var creatureAmount = SavingSystemManager.Instance.GetEnvironmentData().EnemyData
                            .Count(t => t.CreatureType == CreatureType.MOVER);
                        var spawningAmount = Random.Range(0,
                            currentTier.spawnGameDecision.GetMaxSpawningAmount() - creatureAmount);
                        for (int i = 0; i < spawningAmount; i++)
                        {
                            var availableTile = GameFlowManager.Instance.GetEnvManager()
                                .GetRandomAvailableTile();
                            if (availableTile.x.CompareTo(float.NegativeInfinity) == 1)
                            {
                                SavingSystemManager.Instance.OnSpawnMovableEntity(spawnTree, availableTile);
                            }
                        }
                    }
                }
            }
        }
    }
}