using System;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public class AppearComp : MonoBehaviour
    {
        [SerializeField] private GameObject m_Skin;
        [SerializeField] private ParticleSystem _appearVfx;
        [SerializeField] private int _appearDelay;

        private async void OnEnable()
        {
            _appearVfx.Play();
            await SkinAppear();
        }

        private void OnDisable()
        {
            m_Skin.SetActive(false);
        }

        private async Task SkinAppear()
        {
            await Task.Delay(_appearDelay);
            m_Skin.SetActive(true);
        }
    }
}