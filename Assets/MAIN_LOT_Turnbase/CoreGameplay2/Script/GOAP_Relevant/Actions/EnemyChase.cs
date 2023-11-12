using System;
using System.Collections.Generic;
using GOAP;
using JumpeeIsland;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class EnemyChase : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        [SerializeField] private GameObject[] TestTarget;
        [Tooltip("Remove (\")targetAvailable(\") state which is add from collecting phase")]
        [SerializeField] private bool _isContibutePhase;

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
            var availableTarget = _targets.FindAll(t => t.IsCheckable());
            if (availableTarget.Count > 0)
            {
                var selectedTarget = availableTarget[Random.Range(0,availableTarget.Count)].GetGameObject();
                Target = selectedTarget;
                m_GAgent.Inventory.AddItem(selectedTarget);
                m_GAgent.SetIProcessUpdate(this);
            }
            
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