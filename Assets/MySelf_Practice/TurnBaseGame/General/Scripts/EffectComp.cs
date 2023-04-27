using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectComp : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> _attackVFX;

    public void AttackVFX(int vfxIndex)
    {
        _attackVFX[vfxIndex-1]?.Play();
    }
}
