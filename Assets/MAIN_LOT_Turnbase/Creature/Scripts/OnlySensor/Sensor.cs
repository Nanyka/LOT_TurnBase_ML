using UnityEngine;

namespace JumpeeIsland
{
    public enum SensorType
    {
        NONE,
        GOAWAY,
        ATTACK,
        DEFENSE,
        SUPPORT,
        FINDOPPORTUNITY,
        PICKUP
    }
    
    [CreateAssetMenu(fileName = "Sensor", menuName = "JumpeeIsland/Sensor", order = 8)]
    public class Sensor : ScriptableObject
    {
        public int DetectRange;
        public SensorType SensorType;
        private ISensorExecute SensorExecute;

        public int Execute(CreatureData m_CreatureData, Transform m_Transform, EnvironmentManager _envManager,
            CreatureEntity m_Entity, SkillComp skillComp)
        {
            if (SensorExecute == null)
                InitiateSensor();

            return SensorExecute.DecideDirection(m_CreatureData,m_Transform,_envManager,m_Entity,skillComp);
        }

        private void InitiateSensor()
        {
            switch (SensorType)
            {
                case SensorType.GOAWAY:
                    SensorExecute = new GoAwaySensor(DetectRange);
                    break;
                case SensorType.ATTACK:
                    SensorExecute = new AttackSensor();
                    break;
                case SensorType.DEFENSE:
                    SensorExecute = new DefenseSensor();
                    break;
                case SensorType.SUPPORT:
                    SensorExecute = new SupportSensor();
                    break;
                case SensorType.FINDOPPORTUNITY:
                    SensorExecute = new FindOpportunitySensor(DetectRange);
                    break;
                case SensorType.PICKUP:
                    SensorExecute = new PikupSensor();
                    break;
            }
        }
    }
}