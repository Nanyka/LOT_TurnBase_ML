using System.Collections.Generic;
using System.Linq;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class MoveToInProcessArea : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        [Tooltip("Remove (\")targetAvailable(\") state which is add from collecting phase")]

        private BuildingConstructComp[] _testTarget;
        // private readonly List<ICheckableObject> _targets = new(); // TODO: move this list to a distinct manager
        private ICheckableObject _currentPoint;

        // private void Start()
        // {
        //     _testTarget = FindObjectsByType<BuildingConstructComp>(FindObjectsSortMode.None);
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
                var buildings = SavingSystemManager.Instance.GetEnvLoader().GetBuildings(FactionType.Player);
                foreach (var building in buildings)
                {
                    if (building.TryGetComponent(out ICheckableObject checkableObject))
                    {
                        if (checkableObject.IsCheckable())
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
            
            // var availableTarget = _targets.FindAll(t => t.IsCheckable() == false);
            // if (availableTarget.Count > 0)
            // {
            //     Target = availableTarget[Random.Range(0,availableTarget.Count)].GetGameObject();
            //     m_GAgent.SetIProcessUpdate(this);
            // }
            
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