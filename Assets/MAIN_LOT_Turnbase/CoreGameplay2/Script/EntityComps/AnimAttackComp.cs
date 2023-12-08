using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    // public interface IAnimAttack
    
    public class AnimAttackComp : MonoBehaviour, IAttackExecutor
    {
        [SerializeField] private ParticleSystem attackVfx;
        [SerializeField] private List<AnimAttackCollider> attackColliders;
        [SerializeField] private FactionType tempFaction;

        private IAttackComp _successAttack;

        private void Start()
        {
            _successAttack = GetComponent<IAttackComp>();
            
            foreach (var attackCollider in attackColliders)
                attackCollider.Init(this);
        }

        public void PlayAttackVfx()
        {
            attackVfx.Play();
        }

        public FactionType GetFaction()
        {
            return tempFaction;
        }
        
        public void ExecuteHitEffect(GameObject target)
        {
            _successAttack.SuccessAttack(target);
        }
        public void ExecuteHitEffect(int posIndex)
        {
            Debug.Log("Use for multi attacks");
        }

        public void ExecuteHitEffect(Vector3 atPos)
        {
            Debug.Log("Use for single range attack");
        }

        public void ExecuteHitEffect(Vector3 atPos, int skillIndex)
        {
            Debug.Log("Use for multi range attacks");
        }
    }
}
