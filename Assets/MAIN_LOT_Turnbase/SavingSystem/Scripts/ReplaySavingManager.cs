using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public sealed class ReplaySavingManager : SavingSystemManager
    {
        // [SerializeField] private TestBattleRecord m_BattleRecord;

        private FactionReplayController _factionReplay;
        private BuildingReplayController _buildingReplay;
        private int actionIndex;

        protected override void Awake()
        {
            base.Awake();

            var factions = FindObjectsOfType<FactionReplayController>();
            foreach (var faction in factions)
                if (faction.Faction == FactionType.Player)
                    _factionReplay = faction;
            
            _buildingReplay = FindObjectOfType<BuildingReplayController>();
        }

        private void OnDisable()
        {
        }

        public override async void StartLoadData()
        {
            // Authenticate on UGS and get envData
            await m_CloudConnector.Init();

            // Load gameState from local to check if the previous session is disconnected
            LoadPreviousMetadata();
            
            m_EnvLoader.SetData(m_RuntimeMetadata.BattleRecord.environmentData);
            m_EnvLoader.Init();

            GameFlowManager.Instance.OnDataLoaded.Invoke(0);
            Debug.Log("Completed loading process");

            if (m_RuntimeMetadata.BattleRecord.actions.Count > 0)
                Invoke(nameof(Replay), 1f);
            else
                GameFlowManager.Instance.OnGameOver.Invoke(3000);
        }

        private async void Replay()
        {
            await WaitForTheNextAction(3000);
        }

        private async Task WaitForTheNextAction(int waitPeriod)
        {
            Debug.Log($"Delay millisecond: {waitPeriod}");
            await Task.Delay(waitPeriod);
            if (actionIndex < m_RuntimeMetadata.BattleRecord.actions.Count)
            {
                var currentAction = m_RuntimeMetadata.BattleRecord.actions[actionIndex];

                switch (currentAction.EntityType)
                {
                    case EntityType.ENEMY:
                        _factionReplay.MoveCreature(m_RuntimeMetadata.BattleRecord.actions[actionIndex]);
                        break;
                    case EntityType.BUILDING:
                        _buildingReplay.BuildingFire(m_RuntimeMetadata.BattleRecord.actions[actionIndex]);
                        break;
                }
                
                actionIndex++;
                await WaitForTheNextAction(waitPeriod);
            }
            else
                GameFlowManager.Instance.OnGameOver.Invoke(3000);
        }

        public BattleRecord GetBattleRecord()
        {
            return m_RuntimeMetadata.BattleRecord;
        }
    }
}