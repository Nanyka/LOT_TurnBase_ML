using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTutorialAttackVisual : MonoBehaviour
    {
        [SerializeField] private ParticleSystem attackVfx;
        [SerializeField] private ParticleSystem harvestVfx;
        
        public void PlayAttackVfx()
        {
            attackVfx.Play();
        }
        
        public void PlayHarvestVfx()
        {
            harvestVfx.Play();
        }
    }
}