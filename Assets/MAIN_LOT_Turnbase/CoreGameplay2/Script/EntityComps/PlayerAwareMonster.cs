using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private CancellationTokenSource _cancellation = new();
        private bool isDetected;

        private void OnEnable()
        {
            isDetected = false;
        }

        private void OnDisable()
        {
            _cancellation.Cancel();
        }

        private void Awake()
        {
            m_Brain = GetComponent<IBrain>();
            m_Mover = mainEntity.GetComponent<IMover>();
        }

        private void Start()
        {
            GameFlowManager.Instance.OnMonsterAttack.AddListener(AwareMonsterAttack);
        }

        public void AwareMonsterAttack()
        {
            if (isDetected)
                return;

            var target = ExecuteSensor();

            if (target != null)
            {
                m_Mover.StopWalk();
                m_Brain.RefreshBrain(detectedState);
                isDetected = true;
                // await RecheckTarget();
            }
        }

        // private async Task RecheckTarget()
        // {
        //     try
        //     {
        //         _cancellation = new CancellationTokenSource();
        //         await Task.Delay(3000, _cancellation.Token);
        //         if (ExecuteSensor() == null)
        //         {
        //             isDetected = false;
        //             m_Brain.GetBeliefStates().ModifyState(detectedState,-1);
        //         }
        //         else
        //             await RecheckTarget();
        //     }
        //     catch (Exception e)
        //     {
        //         // ignored
        //     }
        // }

        public GameObject ExecuteSensor()
        {
            Collider[] hitColliders = new Collider[10];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, detectRange, hitColliders, layerMask);
            GameObject target = null;

            if (numColliders > 0)
            {
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

                // return target;
            }

            if (target == null)
                ResetSensor();
            
            return target;
        }

        public float DetectRange()
        {
            return detectRange;
        }

        public bool FullyDetect()
        {
            if (isDetected)
                return false;

            var target = ExecuteSensor();

            if (target != null)
            {
                m_Mover.StopWalk();
                m_Brain.RefreshBrain(detectedState);
                isDetected = true;
            }

            return isDetected;
        }

        private void ResetSensor()
        {
            if (!isDetected) return;
            isDetected = false;
            m_Brain.GetBeliefStates().RemoveState(detectedState);
        }
    }
}