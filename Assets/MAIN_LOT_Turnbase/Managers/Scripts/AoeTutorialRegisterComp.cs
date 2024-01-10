using UnityEngine;
using GOAP;

namespace JumpeeIsland
{
    public class AoeTutorialRegisterComp : MonoBehaviour
    {
        [SerializeField] private CreatureData m_CreatureData;
        [SerializeField] private GameObject m_TutorialRegister;
        // [SerializeField] private string m_WorldState;
        // [SerializeField] private bool _isBoss;

        // private void OnDisable()
        // {
        //     if (GWorld.Instance != null)
        //         GWorld.Instance.GetWorld().ModifyState(m_WorldState, -1);
        // }

        public void Register()
        {
            // Register with environment data
            SavingSystemManager.Instance.GetEnvironmentData().EnemyData.Add(m_CreatureData);
            GetComponent<ICreatureInit>().Init(m_CreatureData);
            
            // Initiate the tutorial register
            if (m_TutorialRegister.TryGetComponent(out ITutorialRegister register))
                register.Init();
            
            // // Register with monster controller
            // if (_isBoss)
            //     SavingSystemManager.Instance.GetMonsterController().RegisterBossObject(gameObject);
            //
            // // Add world state, if available
            // if (m_WorldState.Equals("") == false)
            //     GWorld.Instance.GetWorld().ModifyState(m_WorldState, 1);
        }
    }
}