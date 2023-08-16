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
        public GameMasterDecision spawningDecision;
    }

    public class GameSpawner : MonoBehaviour
    {
        [SerializeField] private List<SpawningTier> _spawningGameTiers;
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
            SpawningTier currentTier = _spawningGameTiers[0];
            foreach (var tier in _spawningGameTiers)
            {
                if (tier.tierCondition.CheckPass() == false)
                    break;
                currentTier = tier;
            }
            
            // Select game to spawn if spawning condition is passed
            if (currentTier.spawningDecision.CheckGetThrough() == false)
            {
                var gameList = currentTier.spawningDecision.GetObjectsToSpawn();
                if (gameList != null && gameList.Any())
                {
                    var availableTile = GameFlowManager.Instance.GetEnvManager().GetRandomAvailableTile();
                    if (availableTile.x.CompareTo(float.NegativeInfinity) == 1)
                    {
                        switch (currentTier.spawningDecision.GetEntityType())
                        {
                            case EntityType.COLLECTABLE:
                                SavingSystemManager.Instance.OnSpawnCollectable(
                                    gameList.ElementAt(Random.Range(0, gameList.Count())), availableTile, 0);
                                break;
                            case EntityType.RESOURCE:
                                SavingSystemManager.Instance.OnSpawnResource(
                                    gameList.ElementAt(Random.Range(0, gameList.Count())), availableTile);
                                break;
                            case EntityType.ENEMY:
                                SavingSystemManager.Instance.OnSpawnMovableEntity(
                                    gameList.ElementAt(Random.Range(0, gameList.Count())), availableTile);
                                break;
                        }
                    }
                }
            }
        }
    }
}