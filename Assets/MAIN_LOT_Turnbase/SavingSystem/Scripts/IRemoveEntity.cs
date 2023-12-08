using UnityEngine;

namespace JumpeeIsland
{
    public interface IRemoveEntity
    {
        // public void Remove(IEnvironmentLoader envLoader);
        public GameObject GetRemovedObject();
        public EntityData GetEntityData();
    }
}