using UnityEngine;

namespace JumpeeIsland
{
    public interface ILoadData
    {
        public void StartUpLoadData<T>(T data);
        public GameObject PlaceNewObject<T>(T data);
    }
}