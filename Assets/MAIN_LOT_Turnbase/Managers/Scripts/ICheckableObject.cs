using UnityEngine;

namespace JumpeeIsland
{
    public interface ICheckableObject
    {
        public bool IsCheckable();
        public Vector3 GetPosition();
        public void ReduceCheckableAmount(int amount);
        public GameObject GetGameObject();
    }
}