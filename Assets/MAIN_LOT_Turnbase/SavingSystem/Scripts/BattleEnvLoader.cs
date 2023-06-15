using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public class BattleEnvLoader : EnvironmentLoader
    {
        [SerializeField] private EnvironmentData _playerEnvCache;
        
        public override async void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            Debug.Log("Load data into managers...");
            
            // Save playerEnv in cache used for saving at the end of battle
            _playerEnvCache = _environmentData;
            
            // Load EnemyEnv
            _environmentData = await SavingSystemManager.Instance.GetEnemyEnv();;
            
            // Customize battle env from enemy env and player env
            _environmentData.PrepareForBattleMode(_playerEnvCache.PlayerData);
            
            ExecuteEnvData();
        }
        
        public override EnvironmentData GetDataForSave()
        {
            return _playerEnvCache;
        }
    }
}