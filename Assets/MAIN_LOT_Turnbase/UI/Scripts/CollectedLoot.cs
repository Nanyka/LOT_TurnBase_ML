using System;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class CollectedLoot : MonoBehaviour
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

        private Dictionary<string, int> m_CollectedResource = new();

        private void Start()
        {
            MainUI.Instance.OnShowCurrencyVfx.AddListener(ExecuteEffect);
        }

        private void ExecuteEffect(string currencyId, int amount, Vector3 fromPos)
        {
            if (amount <= 0)
                return;

            m_CollectedResource.TryAdd(currencyId, 0);
            m_CollectedResource[currencyId] += amount;
            collectedAmount.text = m_CollectedResource[currencyId].ToString();

            if (currencyId.Equals("FOOD"))
            {
                foodCollect.SetBurst(0, 0, amount);
                foodCollect.Play();
            }

            if (currencyId.Equals("WOOD"))
            {
                foodCollect.SetBurst(0, 0, amount);
                woodCollect.Play();
            }

            if (currencyId.Equals("COIN"))
            {
                foodCollect.SetBurst(0, 0, amount);
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

        public Dictionary<string, int> GetCollectedLoot()
        {
            return m_CollectedResource;
        }
    }
}