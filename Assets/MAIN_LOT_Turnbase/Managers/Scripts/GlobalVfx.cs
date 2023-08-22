using AssetKits.ParticleImage;
using UnityEngine;

namespace JumpeeIsland
{
    public enum GlobalVfxType
    {
        NONE,
        JUMP,
        FULLSCREENCONFETTI,
    }
    
    public class GlobalVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _jumpVfx;
        [SerializeField] private ParticleImage _fullscreenConfesti;

        public void PlayGlobalVfx(GlobalVfxType type, Vector3 atPos)
        {
            switch (type)
            {
                case GlobalVfxType.JUMP:
                    PlayJump(atPos);
                    break;
            }
            
            switch (type)
            {
                case GlobalVfxType.FULLSCREENCONFETTI:
                    PlayConfetti();
                    break;
            }
        }

        private void PlayJump(Vector3 jumpPos)
        {
            _jumpVfx.transform.position = jumpPos;
            _jumpVfx.Play();
        }
        
        private void PlayConfetti()
        {
            _fullscreenConfesti.Play();
        }
    }
}