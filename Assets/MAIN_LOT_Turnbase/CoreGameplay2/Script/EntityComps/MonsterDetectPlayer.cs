using System;
using System.Threading;
using System.Threading.Tasks;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    // It is a sensor of a monster which detects the nearby enemies.
    public class MonsterDetectPlayer : MonoBehaviour, ISensor
    {
        [SerializeField] private float detectRange;
        [SerializeField] private string detectedState;

        private IBrain m_Brain;
        private LayerMask layerMask = 1 << 7;
        private CancellationTokenSource _cancellation = new();
        private bool isDetected;

        private void Start()
        {
            m_Brain = GetComponent<IBrain>();
            InvokeRepeating(nameof(CheckTroopInRange),3f,3f);
        }
        
        private async void CheckTroopInRange()
        {
            // Check if the inventory is any.
            // If the inventory is empty, adding the nearby enemy into the Inventory, and set the belief that it is an enemy in range

            if (isDetected)
                return;
            
            var target = ExecuteSensor();

            if (target != null)
            {
                m_Brain.RefreshBrain();
                m_Brain.GetBeliefStates().ModifyState(detectedState,1);
                isDetected = true;
                await RecheckTarget();
            }
        }
        
        private async Task RecheckTarget()
        {
            try
            {
                _cancellation = new CancellationTokenSource();
                await Task.Delay(3000, _cancellation.Token);
                if (ExecuteSensor() == null)
                {
                    isDetected = false;
                    m_Brain.GetBeliefStates().ModifyState(detectedState,-1);
                }
                else
                    await RecheckTarget();
            }
            catch (Exception e)
            {
                // ignored
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