using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class FireMovableObj : GAction
    {
        [SerializeField] private GameObject m_Character;
        
        private float _checkDistance;
        private ISensor _detectPlayerTroop;
        private IAttackExecutor m_AttackExecutor;

        private void Start()
        {
            _detectPlayerTroop = GetComponent<ISensor>();
            m_AttackExecutor = m_Character.GetComponent<IAttackExecutor>();
            _checkDistance = _detectPlayerTroop.DetectRange() + 1f;
        }

        public override bool PrePerform()
        {
            var target = _detectPlayerTroop.ExecuteSensor();

            if (target == null)
                return false;

            var position = target.transform.position;
            if (Vector3.Distance(transform.position, position) > _checkDistance)
                return false;

            m_Character.transform.LookAt(new Vector3(position.x, m_Character.transform.position.y, position.z));
            m_AttackExecutor.ExecuteHitEffect(position); // Set target

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}