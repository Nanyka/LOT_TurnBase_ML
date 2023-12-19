using System;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectResource : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Character;
        [SerializeField] private float _checkDistance = 2f;
        
        private ICheckableObject _currentPoint;

        public override bool PrePerform()
        {
            var distanceToTarget = float.PositiveInfinity;
            _currentPoint = null;
            var resources = SavingSystemManager.Instance.GetEnvLoader().GetResources();
            foreach (var resource in resources)
            {
                if (resource.TryGetComponent(out ICheckableObject checkableObject))
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

            if (_currentPoint == null || distanceToTarget > _checkDistance)
                return false;
            
            Target = _currentPoint.GetGameObject();
            m_GAgent.SetIProcessUpdate(this);

            return true;
        }

        public override bool PostPerform()
        {
            // m_GAgent.Inventory.ClearInventory();
            return true;
        }

        public async void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            myTransform.LookAt(new Vector3(targetPos.x, myTransform.position.y, targetPos.z));
            m_Character.StartAttack();
            await WaitToCompleteTheAction();
        }
        
        private async Task WaitToCompleteTheAction()
        {
            await Task.Delay(Mathf.RoundToInt(Duration * 1000));
            StopProcess();
        }

        public void StopProcess()
        {
            m_GAgent.FinishFromOutside();
        }
    }
}