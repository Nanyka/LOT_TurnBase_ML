using System;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeEnvDataRegisterComp : MonoBehaviour
    {
        [SerializeField] private CreatureData m_CreatureData;
        [SerializeField] private string m_WorldState;
        [SerializeField] private bool _isBoss;

        private void OnDisable()
        {
            if (GWorld.Instance != null)
                GWorld.Instance.GetWorld().ModifyState(m_WorldState, -1);
        }

        private void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(Register);
        }

        private void Register()
        {
            // Register with environment data
            SavingSystemManager.Instance.GetEnvironmentData().EnemyData.Add(m_CreatureData);
            GetComponent<ICreatureInit>().Init(m_CreatureData);
            
            // Register with monster controller
            if (_isBoss)
                SavingSystemManager.Instance.GetMonsterController().RegisterBossObject(gameObject);
            
            // Add world state, if available
            if (m_WorldState.Equals("") == false)
                GWorld.Instance.GetWorld().ModifyState(m_WorldState, 1);
        }
    }
}