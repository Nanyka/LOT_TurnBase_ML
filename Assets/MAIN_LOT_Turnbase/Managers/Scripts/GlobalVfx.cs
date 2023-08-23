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
    }
    
    public class GlobalVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _jumpVfx;
        [SerializeField] private ParticleImage _fullscreenConfesti;
        [FormerlySerializedAs("_attackPath")] [SerializeField] private JIAttackPath jiAttackPath;

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

        public void ShowAttackPath(IEnumerable<Vector3> highlightPos)
        {
            jiAttackPath.AttackAt(highlightPos);
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