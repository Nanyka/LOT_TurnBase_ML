using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTowerFireExecutor : MonoBehaviour, IAttackExecutor
    {
        [SerializeField] private float detectRange;
        [SerializeField] private FireComp m_FireComp;
        [SerializeField] private List<AnimAttackCollider> attackColliders;
        
        private IAttackComp _attackComp;
        private IAttackRelated _attackRelated;
        
        private LayerMask layerMask = 1 << 7; // Enemy layer is 5
        
        private void Start()
        {
            _attackComp = GetComponent<IAttackComp>();
            _attackRelated = GetComponent<IAttackRelated>();
            
            foreach (var attackCollider in attackColliders)
                attackCollider.Init(this);
            
            InvokeRepeating(nameof(CheckToFire),5f,5f);
        }
        
        public FactionType GetFaction()
        {
            return _attackRelated.GetFaction();
        }

        private void CheckToFire()
        {
            Collider[] hitColliders = new Collider[10];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, detectRange, hitColliders, layerMask);
            
            // TODO: fire the closest one

            for (int i = 0; i < numColliders; i++)
            {
                m_FireComp.PlayCurveFX(hitColliders[i].transform.position);
            }
        }

        public void ExecuteHitEffect(Vector3 atPos)
        {
            throw new System.NotImplementedException();
        }

        public void ExecuteHitEffect(Vector3 atPos, int skillIndex)
        {
            throw new System.NotImplementedException();
        }

        public void ExecuteHitEffect(GameObject target)
        {
            _attackComp.SuccessAttack(target);
        }
    }
}