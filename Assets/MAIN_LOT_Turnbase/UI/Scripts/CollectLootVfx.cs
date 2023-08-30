using System;
using AssetKits.ParticleImage;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class CollectLootVfx : MonoBehaviour
    {
        [SerializeField] private ParticleImage foodCollect;
        [SerializeField] private ParticleImage woodCollect;
        [SerializeField] private ParticleImage coinCollect;
        [SerializeField] private ShakeSettings _shakeSettings;
        [SerializeField] private Transform _bagTransform;

        [Header("Text appear effect")] [SerializeField]
        private Image currencyIcon;

        [SerializeField] private TextMeshProUGUI collectedAmount;
        [SerializeField] private RectTransform targetTextContainer;
        [SerializeField] private TweenSettings<float> textAppearSettings;

        private int totalFood;
        private int totalWood;
        private int totalCoin;

        private void Start()
        {
            MainUI.Instance.OnShowCurrencyVfx.AddListener(ExecuteEffect);
        }

        private void ExecuteEffect(string currencyId, int amount, Vector3 fromPos)
        {
            if (amount <= 0)
                return;
            
            if (currencyId.Equals("FOOD"))
            {
                foodCollect.SetBurst(0, 0, amount);
                totalFood += amount;
                collectedAmount.text = totalFood.ToString();
                foodCollect.Play();
            }

            if (currencyId.Equals("WOOD"))
            {
                foodCollect.SetBurst(0, 0, amount);
                totalWood += amount;
                collectedAmount.text = totalWood.ToString();
                woodCollect.Play();
            }

            if (currencyId.Equals("COIN"))
            {
                foodCollect.SetBurst(0, 0, amount);
                totalCoin += amount;
                collectedAmount.text = totalCoin.ToString();
                coinCollect.Play();
            }

            if (currencyId.Equals("NONE") == false)
            {
                currencyIcon.sprite = AddressableManager.Instance.GetAddressableSprite(
                    SavingSystemManager.Instance.GetCurrencySprite(currencyId));
                Tween.LocalScale(targetTextContainer, textAppearSettings);
            }
        }

        public void OnCollectLoot()
        {
            Tween.PunchLocalScale(_bagTransform, _shakeSettings);
        }
    }
}