using UnityEngine;

namespace JumpeeIsland
{
    public class OnlySensorCreatureInGame : NPCInGame
    {
        [SerializeField] private OnlySensor _sensor;
        private NPCActionInferer _currenInferer;

        #region START UP OVERRIDE
        
        public override void Awake() { }
        
        public override void OnEnable()
        {
            m_Entity.OnUnitDie.AddListener(UnitDie);
            m_Transform = transform;
        }
        
        public override void Init(CreatureData creatureData, IFactionController playerFaction)
        {
            base.Init(creatureData, playerFaction);
            _sensor.Init();
        }
        
        #endregion
        
        public override void SelfInfer(NPCActionInferer inferer)
        {
            _currenInferer = inferer;
            _sensor.SensorInProcess();
        }

        public override void ResponseAction(int direction)
        {
            InferMoving.Action = direction;
            InferMoving.CurrentPos = m_Transform.position;
            GetPositionByDirection(InferMoving.Action);
            _currenInferer.AddActionToCache(InferMoving);
        }

        public void ResponseAction(int direction, int weight)
        {
            InferMoving.Action = direction;
            InferMoving.CurrentPos = m_Transform.position;
            GetPositionByDirection(InferMoving.Action);
            InferMoving.VoteAmount += weight;
            _currenInferer.AddActionToCache(InferMoving);
        }

        protected override void GetPositionByDirection(int direction)
        {
            var movement = m_FactionController.GetMovementInspector()
                .MovingPath(m_Transform.position, direction, 0, 0);
            InferMoving.TargetPos = movement.returnPos;
            InferMoving.JumpCount = movement.jumpCount;

            if (InferMoving.TargetPos != m_Transform.position)
                InferMoving.Direction = InferMoving.TargetPos - m_Transform.position;
        }
        
        // Just pass over the next agent without any contribution
        public override void AskForAction()
        {
            m_FactionController.WaitForCreature();
        }
    }
}