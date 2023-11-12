using System.Collections.Generic;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class MoveToInProcessArea : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        [SerializeField] private GameObject[] TestTarget;
        [Tooltip("Remove (\")targetAvailable(\") state which is add from collecting phase")]

        private readonly List<ICheckableObject> _targets = new(); // TODO: move this list to a distinct manager

        private void Start()
        {
            foreach (var target in TestTarget)
            {
                if (target.TryGetComponent(out ICheckableObject checkableObject))
                    _targets.Add(checkableObject);
            }
        }

        public override bool PrePerform()
        {
            var availableTarget = _targets.FindAll(t => t.IsCheckable() == false);
            if (availableTarget.Count > 0)
            {
                Target = availableTarget[Random.Range(0,availableTarget.Count)].GetGameObject();
                m_GAgent.SetIProcessUpdate(this);
            }
            
            return true;
        }

        public override bool PostPerform()
        {
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