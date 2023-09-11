using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject _priceTag;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _storageSlider;

        public void ShowHealthBar(bool showPrice, bool showStorage)
        {
            if (showPrice)
                _priceTag.SetActive(true);
            else
                _healthSlider.gameObject.SetActive(true);

            if (showStorage)
                _storageSlider.gameObject.SetActive(true);
        }

        public void UpdateHealthSlider(float value)
        {
            _healthSlider.value = value;
        }

        public void UpdateStorageSlider(float value)
        {
            _storageSlider.value = value;
        }

        public void UpdatePrice(int value)
        {
            _priceText.text = value.ToString();
        }
    }
}