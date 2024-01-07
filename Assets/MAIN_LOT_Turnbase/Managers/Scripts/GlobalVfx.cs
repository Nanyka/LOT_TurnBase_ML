using System.Collections.Generic;
using AssetKits.ParticleImage;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public enum GlobalVfxType
    {
        NONE,
        JUMP,
        FULLSCREENCONFETTI,
        RADAR
    }
    
    public class GlobalVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _jumpVfx;
        [SerializeField] private ParticleImage _fullscreenConfesti;
        [SerializeField] private JIAttackPath JIAttackPath;
        [SerializeField] private ParticleSystem _radarVfx;

        public void PlayGlobalVfx(GlobalVfxType type, Vector3 atPos)
        {
            switch (type)
            {
                case GlobalVfxType.JUMP:
                    PlayJump(atPos);
                    break;
                case GlobalVfxType.RADAR:
                    PlayRadar(atPos);
                    break;
                case GlobalVfxType.FULLSCREENCONFETTI:
                    PlayConfetti();
                    break;
            }
            
            // switch (type)
            // {
            //     case GlobalVfxType.FULLSCREENCONFETTI:
            //         PlayConfetti();
            //         break;
            // }
        }

        public void ShowAttackPath(IEnumerable<Vector3> highlightPos)
        {
            JIAttackPath.AttackAt(highlightPos);
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
        
        private void PlayRadar(Vector3 radarPos)
        {
            _radarVfx.transform.position = radarPos;
            _radarVfx.Play();
        }
    }
}