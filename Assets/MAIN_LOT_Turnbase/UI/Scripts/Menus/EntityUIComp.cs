using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class EntityUIComp : MonoBehaviour, IEntityUIUpdate
    {
        [SerializeField] private CurrencyGroup _priceTag;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _storageSlider;
        [SerializeField] private RotationConstraint _rotationConstraint;
        
        private bool _priceTagState;

        public void ShowBars(bool showPrice, bool showHealth, bool showStorage)
        {
            _priceTagState = showPrice;
            
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

        public void ShowPriceTag(bool isShow)
        {
            _priceTag.gameObject.SetActive(isShow);
        }

        public void UpdateHealthSlider(float value)
        {
            _healthSlider.value = value;
        }

        public void UpdateStorage(float value)
        {
            _storageSlider.value = value;
        }
        
        public void UpdatePriceText(int price)
        {
            _priceText.text = price.ToString();
        }

        public void UpdatePrice(string iconAddress, int cost)
        {
            if (iconAddress.Equals(""))
            {
                _priceTag.gameObject.SetActive(false);
            }
            else
            {
                _priceTag.VisualCurrency(iconAddress, cost);
                _priceTag.gameObject.SetActive(true);
            }
        }

        public void TurnHealthSlider(bool isOn)
        {
            _healthSlider.gameObject.SetActive(isOn);
            if (_priceTagState)
                _priceTag.gameObject.SetActive(isOn);
        }
    }

    public interface IEntityUIUpdate
    {
        public void ShowBars(bool showPrice, bool showHealth, bool showStorage);
        public void ShowPriceTag(bool isShow);
        public void UpdateHealthSlider(float value);
        public void UpdateStorage(float value);
        public void UpdatePriceText(int price);
        public void UpdatePrice(string iconAddress, int cost);
        public void TurnHealthSlider(bool isOn);
    }
}