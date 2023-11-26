using System.Collections.Generic;
using System.Linq;
using GOAP;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class EnemyChaseByName : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        // [FormerlySerializedAs("TestTarget")] 
        // [Tooltip("Remove (\")targetAvailable(\") state which is add from collecting phase")] 
        // [SerializeField] private TowerComp[] _testTarget;
        // private bool _isContributePhase;
        //
        // private readonly List<ICheckableObject> _targets = new(); // TODO: move this list to a distinct manager

        private ICheckableObject _currentPoint;

        // private void Start()
        // {
        //     _testTarget = FindObjectsByType<TowerComp>(FindObjectsSortMode.None);
        //     
        //     foreach (var target in _testTarget)
        //     {
        //         if (target.TryGetComponent(out ICheckableObject checkableObject))
        //             _targets.Add(checkableObject);
        //     }
        // }

        public override bool PrePerform()
        {
            var distanceToTarget = float.PositiveInfinity;
            if (m_GAgent.Inventory.IsEmpty())
            {
                _currentPoint = null;
                var buildings = SavingSystemManager.Instance.GetEnvLoader().GetBuildings(FactionType.Enemy);
                
                foreach (var building in buildings)
                {
                    if (building.TryGetComponent(out ICheckableObject checkableObject))
                    {
                        if (checkableObject.IsCheckable() == false)
                            continue;

                        var curDis = Vector3.Distance(transform.position, checkableObject.GetPosition());
                        if (curDis < distanceToTarget)
                        {
                            distanceToTarget = curDis;
                            _currentPoint = checkableObject;
                        }
                    }
                }

                if (_currentPoint != null)
                    m_GAgent.Inventory.AddItem(_currentPoint.GetGameObject());
            }

            Target = m_GAgent.Inventory.items[0];
            m_GAgent.SetIProcessUpdate(this);

            return true;
            
            // var availableTarget = _targets.FindAll(t => t.IsCheckable());
            // if (availableTarget.Count > 0)
            // {
            //     var selectedTarget = availableTarget[Random.Range(0, availableTarget.Count)].GetGameObject();
            //     Target = selectedTarget;
            //     m_GAgent.Inventory.AddItem(selectedTarget);
            //     m_GAgent.SetIProcessUpdate(this);
            // }
            //
            // return true;
        }

        public override bool PostPerform()
        {
            // if (_isContributePhase)
            // {
            //     m_GAgent.Beliefs.RemoveState("targetAvailable");
            //     m_GAgent.Beliefs.ModifyState("Empty", 1);
            // }

            return true;
        }

        public void MoveToDestination(Transform myTransform, Vector3 targetPos)
        {
            if (Vector3.Distance(myTransform.position, targetPos) < m_Entity.GetStopDistance())
                m_GAgent.FinishFromOutside();
            else
                m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            m_Entity.StopMoving();
            m_GAgent.FinishFromOutside();
        }
    }
}