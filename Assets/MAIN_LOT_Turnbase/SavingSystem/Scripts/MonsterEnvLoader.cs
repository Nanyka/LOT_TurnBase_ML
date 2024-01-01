using UnityEngine;

namespace JumpeeIsland
{
    public class MonsterEnvLoad : BattleEnvLoad
    {
        public override void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            MainUI.Instance.OnEnableInteract.AddListener(AnnounceFinishPlaceCreature);
            Debug.Log("Load data into managers...");
            
            tileManager.Init(_environmentData.mapSize);

            // Save playerEnv into the cache that will be used for saving at the end of battle
            _playerEnvCache = _environmentData;
            
            // Load MonsterEnv
            _environmentData = GameFlowManager.Instance.GetQuest().environmentData.DeepCopy();
            
            // Customize battle env from enemy env and player env
            // _environmentData.PrepareForBossMode(_playerEnvCache.PlayerData);
            
            ExecuteEnvData();
            MainUI.Instance.OnShowDropTroopMenu.Invoke(GetSpawnList());
        }
    }
}