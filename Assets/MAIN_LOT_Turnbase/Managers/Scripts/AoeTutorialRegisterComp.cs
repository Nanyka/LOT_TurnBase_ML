using UnityEngine;
using GOAP;

namespace JumpeeIsland
{
    public class AoeTutorialRegisterComp : MonoBehaviour
    {
        [SerializeField] private CreatureData m_CreatureData;
        [SerializeField] private GameObject m_TutorialRegister;

        public void Register()
        {
            // Register with environment data
            SavingSystemManager.Instance.GetEnvironmentData().EnemyData.Add(m_CreatureData);
            GetComponent<ICreatureInit>().Init(m_CreatureData);
            
            // Initiate the tutorial register
            if (m_TutorialRegister.TryGetComponent(out ITutorialRegister register))
                register.Init();
        }
    }
}