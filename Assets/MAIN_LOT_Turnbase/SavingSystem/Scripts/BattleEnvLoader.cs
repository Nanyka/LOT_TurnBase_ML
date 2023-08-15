using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public class BattleEnvLoader : EnvironmentLoader
    {
        [SerializeField] protected EnvironmentData _playerEnvCache;

        // protected List<JIInventoryItem> _spawnList = new();
        private bool _isFinishPlaceCreatures;
        
        public override async void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            MainUI.Instance.OnEnableInteract.AddListener(AnnounceFinishPlaceCreature);
            Debug.Log("Load data into managers...");
            
            // Save playerEnv into the cache that will be used for saving at the end of battle
            _playerEnvCache = _environmentData;
            
            // Load EnemyEnv
            _environmentData = await SavingSystemManager.Instance.GetEnemyEnv();
            
            // Customize battle env from enemy env and player env
            _environmentData.PrepareForBattleMode(_playerEnvCache.PlayerData);
            
            ExecuteEnvData();
            MainUI.Instance.OnShowDropTroopMenu.Invoke(GetSpawnList());
        }

        protected void AnnounceFinishPlaceCreature()
        {
            // _environmentData.DepositRemainPlayerTroop(_playerEnvCache.PlayerData);
            _isFinishPlaceCreatures = true;
        }
        
        public override EnvironmentData GetData()
        {
            return _environmentData;
        }

        public override EnvironmentData GetDataForSave()
        {
            // _playerEnvCache.RemoveZeroHpPlayerCreatures();
            _playerEnvCache.AbstractInBattleCreatures(_environmentData.PlayerData);
            return _playerEnvCache;
        }

        public List<CreatureData> GetSpawnList()
        {
            return _playerEnvCache.PlayerData;
        }
    }
}