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
        PICKUP,
        STEPASIDE,
        FIREINPLACE
    }
    
    [CreateAssetMenu(fileName = "Sensor", menuName = "JumpeeIsland/Sensor", order = 8)]
    public class Sensor : ScriptableObject
    {
        public int DetectRange;
        public SensorType SensorType;
        public int SensorWeight;
        private ISensorExecute SensorExecute;

        public (int,int) Execute(CreatureData mCreatureData, Transform mTransform, EnvironmentManager envManager,
            CreatureEntity mEntity, SkillComp skillComp)
        {
            if (SensorExecute == null)
                InitiateSensor();

            if (SensorExecute != null)
                return SensorExecute.DecideDirection(mCreatureData, mTransform, envManager, mEntity, skillComp);

            return (0,0);
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
                case SensorType.STEPASIDE:
                    SensorExecute = new StepAside();
                    break;
                case SensorType.FIREINPLACE:
                    SensorExecute = new StandInPlace();
                    break;
            }
        }
    }
}