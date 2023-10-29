using UnityEngine;

namespace JumpeeIsland
{
    public class OnlySensor : MonoBehaviour
    {
        [SerializeField] private Sensor[] m_Sensors;

        private OnlySensorCreatureInGame m_Creature;
        private CreatureEntity m_Entity;
        private CreatureData m_CreatureData;
        private SkillComp m_Skills;
        private EnvironmentData _envData;
        private EnvironmentManager _envManager;
        private Transform m_Transform;
        private int movingIndex;

        public void Init()
        {
            m_Creature = GetComponent<OnlySensorCreatureInGame>();
            m_Entity = GetComponent<CreatureEntity>();
            m_CreatureData = (CreatureData)m_Entity.GetData();
            m_Skills = GetComponent<SkillComp>();
            _envData = SavingSystemManager.Instance.GetEnvironmentData();
            _envManager = GameFlowManager.Instance.GetEnvManager();
            m_Transform = transform;
        }
        
        public void SensorInProcess()
        {
            foreach (var sensor in m_Sensors)
                ResponseToCreature(sensor.Execute(m_CreatureData,m_Transform,_envManager,m_Entity,m_Skills),
                    sensor.SensorWeight);
        }
        
        #region CONNECT TO CREATURE

        private void ResponseToCreature((int,int) action, int weight)
        {
            m_Creature.ResponseAction(action.Item1, action.Item2, weight);
        }

        #endregion
    }
}