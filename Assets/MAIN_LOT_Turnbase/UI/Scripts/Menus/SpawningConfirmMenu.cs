using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class SpawningConfirmMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _menuContainer;
        [SerializeField] private TextMeshProUGUI _entityName;
        [SerializeField] private TextMeshProUGUI _entityDescription;
        [SerializeField] private Image _entityIcon;

        public void ShowMenu(JIInventoryItem inventoryItem)
        {
            _entityName.text = inventoryItem.inventoryName;
            _entityDescription.text = inventoryItem.description;
            _entityIcon.sprite = AddressableManager.Instance.GetAddressableSprite(inventoryItem.spriteAddress);
            
            _menuContainer.SetActive(true);
        }

        public void HideMenu()
        {
            _menuContainer.SetActive(false);
        }
    }
}