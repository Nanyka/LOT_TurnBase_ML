using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class EntityUI : MonoBehaviour
    {
        [SerializeField] private CurrencyGroup _priceTag;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _storageSlider;
        [SerializeField] private RotationConstraint _rotationConstraint;

        public void ShowBars(bool showPrice, bool showHealth, bool showStorage)
        {
            if (showPrice)
                _priceTag.gameObject.SetActive(true);

            if (showHealth)
                _healthSlider.gameObject.SetActive(true);

            if (showStorage)
                _storageSlider.gameObject.SetActive(true);

            var constraintSource = new ConstraintSource();
            constraintSource.sourceTransform = Camera.main.transform;
            constraintSource.weight = 1;
            _rotationConstraint.SetSource(0, constraintSource);
        }

        public void ShowPriceTag()
        {
            _priceTag.gameObject.SetActive(true);
        }

        public void UpdateHealthSlider(float value)
        {
            _healthSlider.value = value;
        }

        public void UpdateStorageSlider(float value)
        {
            _storageSlider.value = value;
        }

        public void UpdatePrice(string iconAddress, int cost)
        {
            if (iconAddress.IsNullOrWhitespace() == false)
            {
                _priceTag.gameObject.SetActive(false);
            }
            else
            {
                _priceTag.VisualCurrency(iconAddress, cost);
                _priceTag.gameObject.SetActive(true);
            }
        }

        public void UpdatePrice(int value)
        {
            _priceText.text = value.ToString();
        }

        public void TurnHealthSlider(bool isOn)
        {
            _healthSlider.gameObject.SetActive(isOn);
            _priceTag.gameObject.SetActive(isOn);
        }
    }
}