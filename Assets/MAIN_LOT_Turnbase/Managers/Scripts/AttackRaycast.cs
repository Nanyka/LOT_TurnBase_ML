using System;
using UnityEditor.VersionControl;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace JumpeeIsland
{
    public class AttackRaycast : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private ParticleSystem _impactVfx;
        [SerializeField] private int _delay;
        
        private async void OnParticleSystemStopped()
        {
            await AttackAlongRaycast();
        }

        private async Task AttackAlongRaycast()
        {
            var mTransform = transform;
            var hits = Physics.RaycastAll(mTransform.position, mTransform.forward, 10f, _layerMask);
            
            foreach (var hit in hits)
            {
                await Task.Delay(_delay);
                _impactVfx.transform.position = hit.transform.position;
                _impactVfx.Play();
            }
        }
    }
}