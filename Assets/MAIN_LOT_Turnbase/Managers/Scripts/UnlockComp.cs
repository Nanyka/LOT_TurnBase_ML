using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class UnlockComp : MonoBehaviour
    {
        [SerializeField] private string _inventoryId;
        
        private void Start()
        {
            GetComponent<Entity>().OnUnitDie.AddListener(Unlock);
        }

        private void Unlock(Entity killBy)
        {
            SavingSystemManager.Instance.GrantInventory(_inventoryId);
        }
    }
}