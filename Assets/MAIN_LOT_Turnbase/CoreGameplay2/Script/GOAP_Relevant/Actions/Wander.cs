using GOAP;
using UnityEngine;
using UnityEngine.AI;

namespace JumpeeIsland
{
    public class Wander : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        [SerializeField] private float wanderRange = 1f;
        
        public override bool PrePerform()
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRange;
            var checkSuccess = NavMesh.SamplePosition(randomPoint, out var hit, wanderRange, NavMesh.AllAreas);
            if (checkSuccess)
                m_GAgent.SetIProcessUpdate(this, hit.position);
            else
                m_GAgent.SetIProcessUpdate(this, transform.position);
            
            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }

        public void StartProcess(Transform myTransform, Vector3 targetPos)
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