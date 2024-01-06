using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ZombieMoveToResource : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;
        
        private ICheckableObject _currentPoint;
        private ISensor _sensor;
        
        private void Start()
        {
            _sensor = GetComponent<ISensor>();
        }

        public override bool PrePerform()
        {
            var target = _sensor.ExecuteSensor();
            if (target == null)
                return false;

            Target = target;
            m_GAgent.SetIProcessUpdate(this);

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }

        public void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            // m_Entity.SetCarryingState(false);
            m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            m_GAgent.FinishFromOutside();
        }
    }
}