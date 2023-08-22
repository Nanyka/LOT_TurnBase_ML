using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("_spawningGameTiers")] [SerializeField]
        private List<SpawningTier> _spawningTiers;

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
                if (currentTier.spawnGameDecision.CheckGetThrough() == false)
                {
                    var gameList = currentTier.spawnGameDecision.GetObjectsToSpawn();
                    if (gameList != null && gameList.Any())
                    {
                        var availableTile = GameFlowManager.Instance.GetEnvManager().GetRandomAvailableTile();
                        if (availableTile.x.CompareTo(float.NegativeInfinity) == 1)
                        {
                            switch (currentTier.spawnGameDecision.GetEntityType())
                            {
                                case EntityType.ENEMY:
                                    SavingSystemManager.Instance.OnSpawnMovableEntity(
                                        gameList.ElementAt(Random.Range(0, gameList.Count())), availableTile);
                                    break;
                            }
                        }
                    }
                }
            }

            // Select game to spawn if spawning condition is passed
            if (currentTier.spawnTreeDecision != null)
            {
                if (currentTier.spawnTreeDecision.CheckGetThrough() == false)
                {
                    var gameList = currentTier.spawnTreeDecision.GetObjectsToSpawn();
                    if (gameList != null && gameList.Any())
                    {
                        var availableTile = GameFlowManager.Instance.GetEnvManager().GetRandomAvailableTile();
                        if (availableTile.x.CompareTo(float.NegativeInfinity) == 1)
                        {
                            switch (currentTier.spawnTreeDecision.GetEntityType())
                            {
                                case EntityType.RESOURCE:
                                    SavingSystemManager.Instance.OnSpawnResource(
                                        gameList.ElementAt(Random.Range(0, gameList.Count())), availableTile);
                                    break;
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
                    var gameList = currentTier.spawnCollectableObjDecision.GetObjectsToSpawn();
                    if (gameList != null && gameList.Any())
                    {
                        var availableTile = GameFlowManager.Instance.GetEnvManager().GetRandomAvailableTile();
                        if (availableTile.x.CompareTo(float.NegativeInfinity) == 1)
                        {
                            switch (currentTier.spawnCollectableObjDecision.GetEntityType())
                            {
                                case EntityType.COLLECTABLE:
                                    SavingSystemManager.Instance.OnSpawnCollectable(
                                        gameList.ElementAt(Random.Range(0, gameList.Count())), availableTile, 0);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}