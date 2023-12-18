using System.Collections.Generic;
using FOW;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTowerFireExecutor : MonoBehaviour, IAttackExecutor, IHiderDisable
    {
        [SerializeField] private float detectRange;
        [SerializeField] private FireComp m_FireComp;
        [SerializeField] private List<AnimAttackCollider> attackColliders;

        private IAttackComp _attackComp;
        private IAttackRelated _attackRelated;
        private IAnimateComp _animateComp;
        // private FogOfWarHider m_Hider;
        private LayerMask layerMask = 1 << 7; // Enemy layer is 5
        private Vector3 _firePos;
        private bool _isDisableHider;

        private void Start()
        {
            _attackComp = GetComponent<IAttackComp>();
            _attackRelated = GetComponent<IAttackRelated>();
            _animateComp = GetComponent<IAnimateComp>();

            foreach (var attackCollider in attackColliders)
                attackCollider.Init(this);

            InvokeRepeating(nameof(CheckToFire), 5f, 5f);
        }

        public FactionType GetFaction()
        {
            return _attackRelated.GetFaction();
        }

        private void CheckToFire()
        {
            Collider[] hitColliders = new Collider[10];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, detectRange, hitColliders, layerMask);

            // Disable hider from the any first fire
            // if (numColliders > 0 && _isDisableHider == false)
            // {
            //     _isDisableHider = true;
            //     m_Hider.OnDisable();
            // }

            var distanceToTarget = float.PositiveInfinity;

            for (int i = 0; i < numColliders; i++)
            {
                var curDis = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                if (curDis < distanceToTarget)
                {
                    distanceToTarget = curDis;
                    _firePos = hitColliders[i].transform.position;
                }
            }

            if (distanceToTarget < float.PositiveInfinity)
                _animateComp.TriggerAttackAnim(0); // TODO: use attackIndex from PowerComp
        }

        public void Fire()
        {
            m_FireComp.PlayCurveFX(_firePos);
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

        public void SetHider(FogOfWarHider hider)
        {
            // m_Hider = hider;
        }
    }

    public interface IHiderDisable
    {
        public void SetHider(FogOfWarHider hider);
    }
}