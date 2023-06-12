using System.Collections.Generic;
using Unity.Services.Economy.Model;
using UnityEngine;

namespace JumpeeIsland
{
    public class InventoryLoader : MonoBehaviour
    {
        private List<JIInventoryItem> m_Inventories;
        
        public void SetData(List<PlayersInventoryItem> inventories)
        {
            m_Inventories = new();

            foreach (var item in inventories)
                m_Inventories.Add(item.GetItemDefinition().CustomDataDeserializable.GetAs<JIInventoryItem>());
        }

        public void SendInventoriesToMenu()
        {
            MainUI.Instance.OnShowBuildingMenu.Invoke(m_Inventories);
        }
    }

    public class JIInventoryItem
    {
        public string inventoryName;
        public InventoryType inventoryType;
        public string spriteAddress;
        public string skinAddress;
    }

    public enum InventoryType
    {
        None,
        Building,
        Unit
    }
}