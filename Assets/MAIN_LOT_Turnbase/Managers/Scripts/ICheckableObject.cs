using UnityEngine;

namespace JumpeeIsland
{
    public interface ICheckableObject
    {
        public bool IsCheckable();
        public Vector3 GetPosition();
        public void ReduceCheckableAmount(int amount);
        public GameObject GetGameObject();
        public int GetRemainAmount();
    }

    public interface IStoreResource
    {
        public bool IsFullStock();
        public void StoreResource(int amount);
        public GameObject GetGameObject();
    }

    public interface IChangeWorldState
    {
        public void ChangeState(int amount);
    }
}