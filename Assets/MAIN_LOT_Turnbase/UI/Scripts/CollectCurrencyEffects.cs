using System;
using AssetKits.ParticleImage;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectCurrencyEffects : MonoBehaviour
    {
        [SerializeField] private ParticleImage _foodVfx;
        [SerializeField] private ParticleImage _woodVfx;
        [SerializeField] private ParticleImage _coinVfx;
        [SerializeField] private ParticleImage _gemVfx;

        private Camera _mainCamera;

        private void Start()
        {
            MainUI.Instance.OnShowCurrencyVfx.AddListener(ShowEffect);
            _mainCamera = Camera.main;
        }

        private void ShowEffect(string currencyId, int amount, Vector3 atPos)
        {
            amount = Mathf.Clamp(amount, 0, 50);
            if (currencyId.Equals("FOOD"))
            {
                _foodVfx.transform.position = _mainCamera.WorldToScreenPoint(atPos);
                _foodVfx.SetBurst(0,0,amount);
                _foodVfx.Play();
            }
            
            if (currencyId.Equals("WOOD"))
            {
                _woodVfx.transform.position = _mainCamera.WorldToScreenPoint(atPos);
                _woodVfx.SetBurst(0,0,amount);
                _woodVfx.Play();
            }
            
            if (currencyId.Equals("COIN"))
            {
                _coinVfx.transform.position = _mainCamera.WorldToScreenPoint(atPos);
                _coinVfx.SetBurst(0,0,amount);
                _coinVfx.Play();
            }
            
            if (currencyId.Equals("GEM"))
            {
                _gemVfx.transform.position = _mainCamera.WorldToScreenPoint(atPos);
                _gemVfx.SetBurst(0,0,amount);
                _gemVfx.Play();
            }
        }
    }
}