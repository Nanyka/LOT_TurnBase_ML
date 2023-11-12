using UnityEngine;

namespace GOAP
{
    public class EnemyIdle : GAction
    {
        private float _originalInterval;

        public override bool PrePerform()
        {
            // Debug.Log("Idle");
            return true;
        }

        public override bool PostPerform()
        {
            m_GAgent.Beliefs.RemoveState("Idle");
            return true;
        }
    }
}
