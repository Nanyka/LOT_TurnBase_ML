using System;
using System.Collections;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace LOT_Turnbase
{
    public class NPCInGame : CreatureInGame, IGetCreatureInfo
    {
        public DummyAction InferMoving;

        private BehaviorParameters m_BehaviorParameters;
        private Agent m_Agent;
        private int _currentDirection;

        public void Awake()
        {
            // Since this example does not inherit from the Agent class, explicit registration
            // of the RpcCommunicator is required. The RPCCommunicator should only be compiled
            // for Standalone platforms (i.e. Windows, Linux, or Mac)
#if UNITY_EDITOR || UNITY_STANDALONE
            if (!CommunicatorFactory.CommunicatorRegistered)
            {
                CommunicatorFactory.Register<ICommunicator>(RpcCommunicator.Create);
            }
#endif
        }

        public new virtual void OnEnable()
        {
            base.OnEnable();

            m_Agent = GetComponent<Agent>();
            m_BehaviorParameters = GetComponent<BehaviorParameters>();
        }

        #region INFER PHASE

        /// <summary>
        ///   <para>Send an action to agent, instead of asking for inferring from a brain, and ask for its reaction</para>
        /// </summary>
        public DummyAction RespondFromAction(int action)
        {
            InferMoving.Action = action;
            InferMoving.CurrentPos = m_Transform.position;
            GetPositionByDirection(InferMoving.Action);
            return InferMoving;
        }

        public void AskForAction()
        {
            m_Agent?.RequestDecision();
        }

        public void ResponseAction(int direction)
        {
            InferMoving.Action = direction;
            InferMoving.CurrentPos = m_Transform.position;
            GetPositionByDirection(InferMoving.Action);
            m_FactionController.WaitForCreature();
        }

        private void GetPositionByDirection(int direction)
        {
            var movement = m_FactionController.GetMovementInspector()
                .MovingPath(m_Transform.position, direction, 0, 0);
            InferMoving.TargetPos = movement.returnPos;
            InferMoving.JumpCount = movement.jumpCount;

            if (InferMoving.TargetPos != m_Transform.position)
                InferMoving.Direction = InferMoving.TargetPos - m_Transform.position;
        }

        public new int GetJumpStep()
        {
            return InferMoving.JumpCount;
        }

        public Vector3 GetDirection()
        {
            return InferMoving.Direction;
        }

        public void SetBrain(NNModel brain)
        {
            m_BehaviorParameters.Model = brain;
        }

        #endregion
        
        #region ACTION PHASE

        public void ConductSelectedAction(DummyAction selectedAction)
        {
            MarkAsUsedThisTurn();
            InferMoving = selectedAction;
        
            // Change agent direction before the agent jump to the new position
            if (selectedAction.TargetPos != m_Transform.position)
                _rotatePart.forward = selectedAction.TargetPos - m_Transform.position;

            StartCoroutine(MoveOverTime());
        }
    
        private IEnumerator MoveOverTime()
        {
            while (transform.position != InferMoving.TargetPos)
            {
                m_Transform.position = Vector3.MoveTowards(transform.position, InferMoving.TargetPos, 10f * Time.deltaTime);
                yield return null;
            }

            // Ask for the next inference
            m_FactionController.KickOffNewTurn();
        }

        public new void Attack()
        {
            m_Entity.AttackSetup(this);
        }

        #endregion
        
        #region GET & SET

        public new (string name, int health, int damage, int power) InfoToShow()
        {
            return (name ,m_Entity.GetCurrentHealth(), m_Entity.GetAttackDamage(), InferMoving.JumpCount);
        }

        public new (Vector3 midPos, Vector3 direction, int jumpStep, FactionType faction) GetCurrentState()
        {
            return (m_Transform.position, _rotatePart.forward, InferMoving.JumpCount, m_FactionController.GetFaction());
        }

        public new EnvironmentManager GetEnvironment()
        {
            return m_FactionController.GetEnvironment();
        }

        public Entity GetEntity()
        {
            return m_Entity;
        }

        protected override void UnitDie()
        {
            base.UnitDie();
            Debug.Log("Do something when NPC die if needed");
        }

        #endregion
    }
}