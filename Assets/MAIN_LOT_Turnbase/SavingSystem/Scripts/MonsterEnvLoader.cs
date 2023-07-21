using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class MonsterEnvLoader : BattleEnvLoader
    {
        [SerializeField] private EnvironmentData _monsterEnv = new();
        
        public override void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            MainUI.Instance.OnEnableInteract.AddListener(AnnounceFinishPlaceCreature);
            Debug.Log("Load data into managers...");
            
            // Save playerEnv into the cache that will be used for saving at the end of battle
            _playerEnvCache = _environmentData;
            
            // Load MonsterEnv
            _environmentData = _monsterEnv;
            
            // Customize battle env from enemy env and player env
            _environmentData.PrepareForBattleMode(_playerEnvCache.PlayerData);
            
            // Send creature data to Creature menu as JIInventoryItem
            // foreach (var creatureData in _playerEnvCache.PlayerData)
            // {
            //     var inventoryItem = creatureData.GetInventoryItem();
            //     inventoryItem.EntityData = creatureData;
            //     _spawnList.Add(inventoryItem);
            // }
            
            ExecuteEnvData();
        }
    }
}