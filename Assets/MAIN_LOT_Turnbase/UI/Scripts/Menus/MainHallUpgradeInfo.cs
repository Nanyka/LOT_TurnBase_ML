using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class MainHallUpgradeInfo : MonoBehaviour
    {
        [SerializeField] private GameObject _infoContainer;
        [SerializeField] private CurrencyGroup _upgradeGroup;
        [SerializeField] private CurrencyButton[] _tierItems;

        public void ShowUpgradeInfo(int upgradePrice)
        {
            foreach (var tierItem in _tierItems)
                tierItem.gameObject.SetActive(false);

            var upcomingTier = SavingSystemManager.Instance.GetUpcomingTier();

            int index = 0;
            foreach (var item in upcomingTier.TierItems)
            {
                if (item.inventoryId.IsNullOrWhitespace())
                    continue;
                
                var inventory = SavingSystemManager.Instance.GetInventoryItemByName(item.itemName);

                if (inventory != null)
                {
                    _tierItems[index].UpdateCurrency($"NEW", inventory.spriteAddress);
                    _tierItems[index].gameObject.SetActive(true);
                    index++;
                }
            }

            _upgradeGroup.VisualCurrency("COIN",upgradePrice);
            _infoContainer.SetActive(true);
        }
    }
}