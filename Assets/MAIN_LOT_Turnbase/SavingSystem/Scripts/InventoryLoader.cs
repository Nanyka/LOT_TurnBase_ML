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

        public void SendInventoriesToBuildingMenu()
        {
            MainUI.Instance.OnBuyBuildingMenu.Invoke(m_Inventories);
        }
        
        public void SendInventoriesToCreatureMenu()
        {
            MainUI.Instance.OnShowCreatureMenu.Invoke(m_Inventories);
        }
        
        public JIInventoryItem GetInventoriesByType(InventoryType inventoryType)
        {
            return m_Inventories.Find(t => t.inventoryType == inventoryType);
        }
    }

    public class JIInventoryItem
    {
        public string inventoryName;
        public InventoryType inventoryType; // To decide which category this inventory item is
        public string spriteAddress;
        public string virtualPurchaseId; // How much does it cost to place this item in game
        public string skinAddress;
    }

    public enum InventoryType
    {
        None,
        Building,
        Creature,
        Resource
    }
}