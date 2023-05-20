using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class EffectComp : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> _attackVFX;

        public void AttackVFX(int vfxIndex)
        {
            _attackVFX[vfxIndex-1]?.Play();
        }
    }
}