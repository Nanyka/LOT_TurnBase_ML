using AssetKits.ParticleImage;
using PrimeTween;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeCollectedLoot : MonoBehaviour
    {
        [SerializeField] private ParticleImage troopCollect;
        [SerializeField] private ParticleImage manaCollect;
        [SerializeField] private ParticleImage coinCollect;
        [SerializeField] private ShakeSettings _shakeSettings;
        
        private Camera _mainCamera;

        private void Start()
        {
            MainUI.Instance.OnShowCurrencyVfx.AddListener(ExecuteEffect);
            _mainCamera = Camera.main;
        }

        private void ExecuteEffect(string currencyId, int amount, Vector3 fromPos)
        {
            if (amount <= 0)
                return;

            if (currencyId.Equals("FOOD"))
            {
                troopCollect.transform.position = _mainCamera.WorldToScreenPoint(fromPos);
                troopCollect.SetBurst(0, 0, amount);
                troopCollect.Play();
            }

            else if (currencyId.Equals("WOOD"))
            {
                manaCollect.transform.position = _mainCamera.WorldToScreenPoint(fromPos);
                manaCollect.SetBurst(0, 0, amount);
                manaCollect.Play();
            }

            else if (currencyId.Equals("COIN"))
            {
                coinCollect.transform.position = _mainCamera.WorldToScreenPoint(fromPos);
                coinCollect.SetBurst(0, 0, amount);
                coinCollect.Play();
            }
        }
        
        public void OnCollectLoot(Transform icon)
        {
            Tween.PunchLocalScale(icon, _shakeSettings);
        }
    }
}