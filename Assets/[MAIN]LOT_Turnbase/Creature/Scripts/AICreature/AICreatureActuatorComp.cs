using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace LOT_Turnbase
{
    public class AICreatureActuatorComp : ActuatorComponent
    {
        [SerializeField] private NPCInGame _controller;
        private ActionSpec _actionSpec = ActionSpec.MakeDiscrete(5, 3);

        public override IActuator[] CreateActuators()
        {
            return new IActuator[] {new AICreatureActuator(_controller)};
        }

        public override ActionSpec ActionSpec
        {
            get { return _actionSpec; }
        }
    }
    
    public class AICreatureActuator : IActuator
    {
        private NPCInGame _controller;
        ActionSpec m_ActionSpec;
    
        public AICreatureActuator(NPCInGame controller)
        {
            _controller = controller;
            m_ActionSpec = ActionSpec.MakeDiscrete(5,3);
        }
    
        public ActionSpec ActionSpec
        {
            get { return m_ActionSpec; }
        }
    
        /// <inheritdoc/>
        public String Name
        {
            get { return "JumpOver"; }
        }
    
        public void ResetData()
        {
        }
    
        public void OnActionReceived(ActionBuffers actionBuffers)
        {
            _controller.ResponseAction(actionBuffers.DiscreteActions[0]);
        }
    
        public void Heuristic(in ActionBuffers actionBuffersOut)
        {
            var directionX = Input.GetAxis("Horizontal");
            var directionZ = Input.GetAxis("Vertical");
            var discreteActions = actionBuffersOut.DiscreteActions;
    
            if (Mathf.Approximately(directionX, 0.0f) && Mathf.Approximately(directionZ, 0.0f))
            {
                discreteActions[0] = 0;
            }
            else if (Mathf.Approximately(directionZ, 0.0f))
            {
                var signX = Mathf.Sign(directionX);
                discreteActions[0] = signX < 0 ? 1 : 2;
            }
            else if (Mathf.Approximately(directionX, 0.0f))
            {
                var signZ = Mathf.Sign(directionZ);
                discreteActions[0] = signZ < 0 ? 3 : 4;
            }
        }
    
        public void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
        }
    }
}
