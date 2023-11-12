using JumpeeIsland;
using UnityEngine;

namespace GOAP
{
    public class EnemyChase : GAction, IProcessUpdate
    {
        [SerializeField] private CreatureEntity m_Entity;
        [SerializeField] private GameObject[] TestTarget;
        [Tooltip("Remove (\")targetAvailable(\") state which is add from collecting phase")]
        [SerializeField] private bool _isContibutePhase;

        public override bool PrePerform()
        {
            Target = TestTarget[Random.Range(0,TestTarget.Length)];
            m_GAgent.SetIProcessUpdate(this);
            return true;
        }

        public override bool PostPerform()
        {
            if (_isContibutePhase)
            {
                m_GAgent.Beliefs.RemoveState("targetAvailable");
                m_GAgent.Beliefs.ModifyState("Empty",1);
            }
            
            return true;
        }

        public void MoveToDestination(Transform myTransform, Vector3 targetPos)
        {
            m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            m_Entity.StopMoving();
            m_GAgent.FinishFromOutside();
        }
    }
}