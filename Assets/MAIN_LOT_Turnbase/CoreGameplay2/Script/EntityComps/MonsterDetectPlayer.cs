using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    // It is a sensor of a monster which detects the nearby enemies.
    public class MonsterDetectPlayer : MonoBehaviour, ISensor
    {
        [SerializeField] private float detectRange;
        [SerializeField] private string detectedState;

        private IBrain brain;
        private LayerMask layerMask = 1 << 7;

        private void Start()
        {
            brain = GetComponent<IBrain>();
            InvokeRepeating(nameof(CheckTroopInRange),5f,5f);
        }
        
        private void CheckTroopInRange()
        {
            if (brain.GetInventory().IsEmpty() == false)
                return;
            // Check if the inventory is any.
            // If the inventory is empty, adding the nearby enemy into the Inventory, and set the belief that it is an enemy in range

            var target = ExecuteSensor();

            if (target != null)
            {
                brain.GetInventory().AddItem(target);
                brain.GetBeliefStates().ModifyState(detectedState,1);
            }
        }

        public GameObject ExecuteSensor()
        {
            Collider[] hitColliders = new Collider[10];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, detectRange, hitColliders, layerMask);

            if (numColliders > 0)
            {
                var target = hitColliders[0].gameObject;
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

    public interface ISensor
    {
        public GameObject ExecuteSensor();
    }
}