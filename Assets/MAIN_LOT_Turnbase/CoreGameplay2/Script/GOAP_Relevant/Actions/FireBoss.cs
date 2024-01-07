using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class FireBoss : GAction
    {
        [SerializeField] private GameObject m_Character;

        private float _checkDistance;
        private IAttackExecutor m_AttackExecutor;
        private ISensor _detectPlayerTroop;
        private Vector3 _currentTarget;

        private void Start()
        {
            m_AttackExecutor = m_Character.GetComponent<IAttackExecutor>();
            _detectPlayerTroop = GetComponent<ISensor>();
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
            
            // var distanceToTarget = float.PositiveInfinity;
            // var monsters = SavingSystemManager.Instance.GetMonsterController().GetMonsters();
            //
            // foreach (var monster in monsters)
            // {
            //     var curDis = Vector3.Distance(transform.position, monster.transform.position);
            //     if (curDis > _checkDistance)
            //         return false;
            //
            //     if (curDis < distanceToTarget)
            //     {
            //         distanceToTarget = curDis;
            //         _currentTarget = monster.transform.position;
            //     }
            // }
            //
            // if (distanceToTarget < float.PositiveInfinity)
            // {
            //     m_Character.transform.LookAt(new Vector3(_currentTarget.x, m_Character.transform.position.y,
            //         _currentTarget.z));
            //     m_AttackExecutor.ExecuteHitEffect(_currentTarget); // Set target
            //     return true;
            // }
            // else
            //     return false;
        }

        public override bool PostPerform()
        {
            // m_GAgent.Inventory.ClearInventory();
            return true;
        }
    }
}