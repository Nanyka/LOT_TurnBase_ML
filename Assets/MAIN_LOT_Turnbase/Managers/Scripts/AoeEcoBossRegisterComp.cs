using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeEcoBossRegisterComp : MonoBehaviour
    {
        [SerializeField] private CreatureData m_CreatureData;
        
        private void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(Register);
        }

        private void Register()
        {
            SavingSystemManager.Instance.GetEnvironmentData().EnemyData.Add(m_CreatureData);
            GetComponent<ICreatureInit>().Init(m_CreatureData);
        }
    }
}