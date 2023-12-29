using UnityEngine;

namespace JumpeeIsland
{
    public class AoeBossEnvLoader : AoeEnvironmentLoader
    {
        [SerializeField] private EnvironmentData _bossEnvData;
        
        public override void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            Debug.Log("Load data into managers...");

            // Save playerEnv into the cache that will be used for saving at the end of battle
            _playerEnvCache = _environmentData;

            // Load EnemyEnv
            _environmentData = _bossEnvData;

            // Customize battle env from enemy env and player env
            _environmentData.PrepareForBattleMode(_playerEnvCache.PlayerData);

            // Update currency UI
            MainUI.Instance.OnUpdateCurrencies.Invoke();

            ExecuteEnvData();
            // Create environment-relevant objects that not include in the saving data
            GetComponent<IEnvironmentCreator>().CreateEnvObjects();

            GameFlowManager.Instance.OnKickOffEnv.Invoke();
            Debug.Log("----GAME START!!!----");
        }
    }
}