using UnityEngine;

namespace GOAP
{
    public interface IProcessUpdate
    {
        public void StartProcess(Transform myTransform ,Vector3 targetPos);
        public void StopProcess();
    }
}