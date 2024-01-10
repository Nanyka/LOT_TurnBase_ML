using System.Collections.Generic;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class CheckInProcessArea : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Character;
        [SerializeField] private float _checkDistance = 1f;
        
        private ICheckableObject _currentPoint;

        public override bool PrePerform()
        {
            var distanceToTarget = float.PositiveInfinity;
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

            if (_currentPoint == null)
                return false;
            
            distanceToTarget = Vector3.Distance(_currentPoint.GetPosition(), transform.position);
            
            if (distanceToTarget < _checkDistance)
            {
                // _currentPoint.ReduceCheckableAmount(1);
                // Duration = 1f;

                m_GAgent.Beliefs.RemoveState("Empty");
                m_GAgent.Beliefs.AddState("targetAvailable");
                
                Target = _currentPoint.GetGameObject();
                m_GAgent.SetIProcessUpdate(this);
            }
            else
            {
                Duration = 0f;
                return false;
            }

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
        
        public async void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            Debug.Log("Start constructing");
            myTransform.LookAt(new Vector3(targetPos.x, myTransform.position.y, targetPos.z));
            m_Character.StartAttack();
            await WaitToCompleteTheAction();
        }
        
        private async Task WaitToCompleteTheAction()
        {
            await Task.Delay(3000);
            StopProcess();
        }

        public void StopProcess()
        {
            Debug.Log("End constructing");

            _currentPoint.ReduceCheckableAmount(1);
            m_GAgent.FinishFromOutside();
        }
    }
}