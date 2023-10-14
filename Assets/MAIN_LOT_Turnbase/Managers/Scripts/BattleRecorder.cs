using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class BattleRecorder : MonoBehaviour
    {
        // [SerializeField] private TestBattleRecord m_BattleRecord;
        private BattleRecord m_BattleRecord = new();

        private void Start()
        {
            SavingSystemManager.Instance.OnRecordAction.AddListener(RecordBattleAction);
            GameFlowManager.Instance.OnKickOffEnv.AddListener(RecordEnvironment);
        }

        private void RecordEnvironment()
        {
            m_BattleRecord.environmentData = new EnvironmentData(SavingSystemManager.Instance.GetEnvironmentData());
        }

        private void RecordBattleAction(RecordAction action)
        {
            m_BattleRecord.actions.Add(action);
        }

        public BattleRecord GetBattleRecord()
        {
            return m_BattleRecord;
        }
    }
}