using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class PlayerAwareMonster : MonoBehaviour, ISensor
    {
        [SerializeField] private GameObject mainEntity;
        [SerializeField] private float detectRange;
        [SerializeField] private string detectedState;
        
        private IBrain m_Brain;
        private IMover m_Mover;
        private LayerMask layerMask = 1 << 11;

        private void Start()
        {
            m_Brain = GetComponent<IBrain>();
            m_Mover = mainEntity.GetComponent<IMover>();
            GameFlowManager.Instance.OnMonsterAttack.AddListener(AwareMonsterAttack);
        }

        private void AwareMonsterAttack()
        {
            var target = ExecuteSensor();

            if (target != null)
            {
                m_Mover.StopWalk();
                m_Brain.RefreshBrain();
                m_Brain.GetInventory().AddItem(target);
                m_Brain.GetBeliefStates().ModifyState(detectedState,1);
            }
        }

        public GameObject ExecuteSensor()
        {
            Collider[] hitColliders = new Collider[10];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, detectRange, hitColliders, layerMask);

            if (numColliders > 0)
            {
                GameObject target = null;
                var distanceToTarget = float.PositiveInfinity;

                for (int i = 0; i < numColliders; i++)
                {
                    var curDis = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                    if (curDis < distanceToTarget)
                    {
                        distanceToTarget = curDis;
                        target = hitColliders[i].gameObject;
                    }
                }

                return target;
            }

            return null;
        }
    }
}