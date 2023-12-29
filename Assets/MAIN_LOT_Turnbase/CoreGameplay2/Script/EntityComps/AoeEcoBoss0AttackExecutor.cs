using UnityEngine;

namespace JumpeeIsland
{
    public class AoeEcoBoss0AttackExecutor : MonoBehaviour, IAttackExecutor
    {
        [SerializeField] private float attackRange;
        [SerializeField] private ParticleSystem[] attackVfx;
        [SerializeField] private AnimAttackCollider attackCollider;
        [SerializeField] private Transform specialAttackPos;

        private IAttackComp _attackComp;
        private IAttackRelated _attackRelated;
        private IComboReceiver _combo;
        private IComboMonitor _comboMonitor;
        private LayerMask layerMask = 1 << 7;

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
            attackCollider.Init(this);
            if (_attackRelated.GetFaction() == FactionType.Player)
                layerMask = 1 << 9 | 1 << 11;
            else
                layerMask = 1 << 9 | 1 << 7;
        }

        public void PlayAttackVfx()
        {
            attackVfx[_combo.GetAttackIndex()].Play();

            if (_combo.GetAttackIndex() > 0)
            {
                Collider[] hitColliders = new Collider[10];
                int numColliders =
                    Physics.OverlapSphereNonAlloc(specialAttackPos.position, attackRange, hitColliders, layerMask);

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