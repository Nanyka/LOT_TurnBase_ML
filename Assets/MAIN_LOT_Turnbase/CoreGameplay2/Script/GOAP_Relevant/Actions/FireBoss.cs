using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class FireBoss : GAction
    {
        [SerializeField] private GameObject m_Character;
        
        private float _checkDistance;
        private IAttackExecutor m_AttackExecutor;
        private Vector3 _currentTarget;

        private void Start()
        {
            m_AttackExecutor = m_Character.GetComponent<IAttackExecutor>();
            _checkDistance = m_GAgent.GetComponent<ISensor>().DetectRange() + 1f;
        }

        public override bool PrePerform()
        {
            // _currentPoint = null;
            // var distanceToTarget = float.PositiveInfinity;
            // var buildings = SavingSystemManager.Instance.GetEnvLoader().GetBuildings(FactionType.Enemy);
            //     
            // foreach (var building in buildings)
            // {
            //     if (building.TryGetComponent(out ICheckableObject checkableObject))
            //     {
            //         if (checkableObject.IsCheckable() == false)
            //             continue;
            //
            //         var curDis = Vector3.Distance(transform.position, checkableObject.GetPosition());
            //         if (curDis < distanceToTarget)
            //         {
            //             distanceToTarget = curDis;
            //             _currentPoint = checkableObject;
            //         }
            //     }
            // }
            //
            // if (_currentPoint == null || distanceToTarget > _checkDistance)
            // {
            //     return false;
            // }
            //
            // var position = _currentPoint.GetPosition();
            
            var distanceToTarget = float.PositiveInfinity;
            var monsters = SavingSystemManager.Instance.GetMonsterController().GetMonsters();

            foreach (var monster in monsters)
            {
                var curDis = Vector3.Distance(transform.position, monster.transform.position);
                if (curDis < distanceToTarget)
                {
                    distanceToTarget = curDis;
                    _currentTarget = monster.transform.position;
                }
            }

            if (distanceToTarget < float.PositiveInfinity)
            {
                m_Character.transform.LookAt(new Vector3(_currentTarget.x, m_Character.transform.position.y,
                    _currentTarget.z));
                m_AttackExecutor.ExecuteHitEffect(_currentTarget); // Set target
                return true;
            }
            else
                return false;
        }

        public override bool PostPerform()
        {
            // m_GAgent.Inventory.ClearInventory();
            return true;
        }
    }
}