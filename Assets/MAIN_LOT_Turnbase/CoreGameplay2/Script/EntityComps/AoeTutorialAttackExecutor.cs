using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTutorialAttackExecutor : MonoBehaviour, IAttackExecutor
    {
        // [SerializeField] private float attackRange;
        [SerializeField] private ParticleSystem attackVfx;
        // [SerializeField] private ParticleSystem harvestVfx;
        // [SerializeField] private Transform specialAttackPos;

        // private IAttackComp _attackComp;
        private IAttackRelated _attackRelated;

        // private IComboReceiver _combo;
        // private IComboMonitor _comboMonitor;
        private LayerMask _attackLayerMask = 1 << 7 | 1 << 9;
        private LayerMask _collectLayerMask = 1 << 8;

        public void Start()
        {
            // _attackComp = GetComponent<IAttackComp>();
            _attackRelated = GetComponent<IAttackRelated>();
            // _combo = GetComponent<IComboReceiver>();
            // _comboMonitor = GetComponent<IComboMonitor>();

            SetAttackLayer();
        }

        private void SetAttackLayer()
        {
            if (attackVfx.TryGetComponent(out AnimAttackCollider attackCollider))
                attackCollider.Init(this);
        }

        // public void PlayHarvestVfx()
        // {
        //     harvestVfx.Play();
        // }

        public void PlayAttackVfx()
        {
            attackVfx.Play();
        }

        public FactionType GetFaction()
        {
            return _attackRelated.GetFaction();
        }

        public void ExecuteHitEffect(GameObject target)
        {
            if (target.TryGetComponent(out IHealthComp healthComp))
            {
                Debug.Log($"Attack an object with a healComp. My attacker: {_attackRelated}");
                healthComp.TakeDamage(_attackRelated);
            }
            
            // _attackComp.SuccessAttack(target);
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