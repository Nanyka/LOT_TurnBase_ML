using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class CurrencyGroup : MonoBehaviour
    {
        [SerializeField] private Image _costIcon;
        [SerializeField] private TextMeshProUGUI _costText;

        public void VisualCurrency(string iconAddress, int cost)
        {
            _costIcon.sprite = AddressableManager.Instance.GetAddressableSprite(
                    SavingSystemManager.Instance.GetCurrencySprite(iconAddress));
            _costIcon.gameObject.SetActive(true);
            _costText.text = cost.ToString();
        }

        public void VisualCurrency(string message)
        {
            _costText.text = message;
            _costIcon.gameObject.SetActive(false);
        }
    }
}