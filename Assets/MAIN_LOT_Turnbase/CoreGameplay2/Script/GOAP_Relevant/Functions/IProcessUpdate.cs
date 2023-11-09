using UnityEngine;

namespace GOAP
{
    public interface IProcessUpdate
    {
        public void MoveToDestination(Transform myTransform ,Vector3 targetPos);
        public void StopProcess();
    }
}