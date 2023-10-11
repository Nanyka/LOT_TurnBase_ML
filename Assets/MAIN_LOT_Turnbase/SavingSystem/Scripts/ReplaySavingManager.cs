using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public sealed class ReplaySavingManager : SavingSystemManager
    {
        [SerializeField] private TestBattleRecord m_BattleRecord;

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

        public override async void StartUpLoadData()
        {
            // Authenticate on UGS and get envData
            await m_CloudConnector.Init();

            // Load gameState from local to check if the previous session is disconnected
            await LoadPreviousMetadata();

            m_EnvLoader.SetData(m_BattleRecord.BattleRecord.environmentData);
            m_EnvLoader.Init();

            GameFlowManager.Instance.OnStartGame.Invoke(0);
            Debug.Log("Completed loading process");

            if (m_BattleRecord.BattleRecord.Actions.Count > 0)
                Invoke(nameof(Replay), 1f);
            else
                GameFlowManager.Instance.OnGameOver.Invoke(3000);
        }

        private async void Replay()
        {
            await WaitForTheNextAction(2000);
        }

        private async Task WaitForTheNextAction(int waitPeriod)
        {
            Debug.Log($"Delay millisecond: {waitPeriod}");
            await Task.Delay(waitPeriod);
            if (actionIndex < m_BattleRecord.BattleRecord.Actions.Count)
            {
                var currentAction = m_BattleRecord.BattleRecord.Actions[actionIndex];

                switch (currentAction.EntityType)
                {
                    case EntityType.ENEMY:
                        _factionReplay.MoveCreature(m_BattleRecord.BattleRecord.Actions[actionIndex]);
                        break;
                    case EntityType.BUILDING:
                        _buildingReplay.BuildingFire(m_BattleRecord.BattleRecord.Actions[actionIndex]);
                        break;
                }

                // if (actionIndex + 1 == m_BattleRecord.BattleRecord.Actions.Count)
                //     waitPeriod = Mathf.RoundToInt(m_BattleRecord.BattleRecord.Actions[actionIndex].AtSecond * 1000);
                // else
                //     waitPeriod = Mathf.RoundToInt((m_BattleRecord.BattleRecord.Actions[actionIndex + 1].AtSecond -
                //                                    m_BattleRecord.BattleRecord.Actions[actionIndex].AtSecond) * 1000);

                actionIndex++;
                await WaitForTheNextAction(waitPeriod);
            }
            else
                GameFlowManager.Instance.OnGameOver.Invoke(3000);
        }

        public BattleRecord GetBattleRecord()
        {
            return m_BattleRecord.BattleRecord;
        }
    }
}