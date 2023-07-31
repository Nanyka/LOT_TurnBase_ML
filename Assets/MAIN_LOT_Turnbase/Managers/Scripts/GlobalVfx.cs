using UnityEngine;

namespace JumpeeIsland
{
    public enum GlobalVfxType
    {
        NONE,
        JUMP
    }
    
    public class GlobalVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _jumpVfx;

        public void PlayGlobalVfx(GlobalVfxType type, Vector3 atPos)
        {
            switch (type)
            {
                case GlobalVfxType.JUMP:
                    PlayJump(atPos);
                    break;
            }
        }

        private void PlayJump(Vector3 jumpPos)
        {
            _jumpVfx.transform.position = jumpPos;
            _jumpVfx.Play();
        }
    }
}