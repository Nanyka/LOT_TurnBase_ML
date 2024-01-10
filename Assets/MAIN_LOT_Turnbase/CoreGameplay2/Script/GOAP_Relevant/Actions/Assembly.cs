using GOAP;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class Assembly : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        [SerializeField] private float randomRadius = 1f;

        public override bool PrePerform()
        {
            var checkSuccess = NavMesh.SamplePosition(m_Entity._assemblyPoint + Random.insideUnitSphere * randomRadius,
                out var hit, randomRadius, NavMesh.AllAreas);
            if (checkSuccess)
                m_GAgent.SetIProcessUpdate(this, hit.position);
            else
                m_GAgent.SetIProcessUpdate(this, transform.position);

            return true;
        }

        public override bool PostPerform()
        {
            m_GAgent.Beliefs.RemoveState("HaveJustWentOut");
            return true;
        }

        public void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            m_GAgent.FinishFromOutside();
        }
    }
}