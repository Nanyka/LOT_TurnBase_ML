using UnityEngine;

namespace JumpeeIsland
{
    public class AoeZombieManualAttackExecutor : MonoBehaviour, IAttackExecutor, IAttackRegister
    {
        [SerializeField] private float attackRange;
        [SerializeField] private ParticleSystem[] attackVfx;
        [SerializeField] private ParticleSystem harvestVfx;
        [SerializeField] private Transform specialAttackPos;

        private IAttackComp _attackComp;
        private IAttackRelated _attackRelated;
        private IComboReceiver _combo;
        private IComboMonitor _comboMonitor;
        private LayerMask _attackLayerMask = 1 << 7 | 1 << 9;
        private LayerMask _collectLayerMask = 1 << 8;

        public void Init()
        {
            _attackComp = GetComponent<IAttackComp>();
            _attackRelated = GetComponent<IAttackRelated>();
            _combo = GetComponent<IComboReceiver>();
            _comboMonitor = GetComponent<IComboMonitor>();

            SetAttackLayer();
        }

        private void SetAttackLayer()
        {
            foreach (var attackChecker in attackVfx)
                if (attackChecker.TryGetComponent(out AnimAttackCollider attackCollider))
                    attackCollider.Init(this);

            if (harvestVfx.TryGetComponent(out AnimAttackCollider harvestCollider))
                harvestCollider.Init(this);
        }

        public void PlayHarvestVfx()
        {
            harvestVfx.Play();
        }

        public void PlayAttackVfx()
        {
            attackVfx[_combo.GetAttackIndex()].Play();

            if (_combo.GetAttackIndex() > 0)
            {
                Collider[] hitColliders = new Collider[10];
                int numColliders =
                    Physics.OverlapSphereNonAlloc(specialAttackPos.position, attackRange, hitColliders, _attackLayerMask);

                if (numColliders > 0)
                {
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider == null)
                            continue;
                        ExecuteHitEffect(hitCollider.gameObject);
                    }
                }
            }

            _comboMonitor.ResetPowerBar();
        }

        public FactionType GetFaction()
        {
            return _attackRelated.GetFaction();
        }

        public void ExecuteHitEffect(GameObject target)
        {
            _attackComp.SuccessAttack(target);
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