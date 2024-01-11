using UnityEngine;

namespace JumpeeIsland
{
    public class TutorialZombieAware : MonoBehaviour, ISensor, ITutorialRegister
    {
        [SerializeField] private float detectRange;
        [SerializeField] private string detectedResourceState;
        [SerializeField] private string detectedEnemyState;

        private IBrain m_Brain;
        private LayerMask unitLayerMask = 1 << 7;
        private LayerMask resourceLayerMask = 1 << 8;
        private ZombieDetectType _currentDetectType = ZombieDetectType.NONE;
        private bool isDetected;

        public void Init()
        {
            m_Brain = GetComponent<IBrain>();
            InvokeRepeating(nameof(CheckTroopInRange),3f,3f);
        }
        
        private void CheckTroopInRange()
        {
            if (isDetected)
                return;
            
            var target = ExecuteSensor();

            if (target != null)
            {
                switch (_currentDetectType)
                {
                    case ZombieDetectType.RESOURCE:
                        m_Brain.RefreshBrain(detectedResourceState);
                        break;
                    case ZombieDetectType.ENEMY:
                        m_Brain.RefreshBrain(detectedEnemyState);
                        break;
                }
                
                isDetected = true;
            }
        }

        public GameObject ExecuteSensor()
        {
            GameObject target = null;

            switch (_currentDetectType)
            {
                case ZombieDetectType.NONE:
                {
                    DetectEnemy(ref target);
                    if (target == null)
                        DetectResource(ref target);
                    break;
                }
                case ZombieDetectType.RESOURCE:
                {
                    DetectResource(ref target);
                    break;
                }
                case ZombieDetectType.ENEMY:
                {
                    DetectEnemy(ref target);
                    break;
                }
            }

            if (target == null)
                ResetSensor();

            return target;
        }

        private void DetectResource(ref GameObject target)
        {
            Collider[] hitColliders = new Collider[1];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, detectRange, hitColliders, resourceLayerMask);
            if (numColliders > 0)
            {
                _currentDetectType = ZombieDetectType.RESOURCE;
                target = hitColliders[0].gameObject;
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
            }
        }

        private void DetectEnemy(ref GameObject target)
        {
            Collider[] hitColliders = new Collider[10];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, detectRange, hitColliders, unitLayerMask);

            if (numColliders > 0)
            {
                _currentDetectType = ZombieDetectType.ENEMY;
                target = hitColliders[0].gameObject;
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
            }
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
                switch (_currentDetectType)
                {
                    case ZombieDetectType.RESOURCE:
                        m_Brain.RefreshBrain(detectedResourceState);
                        break;
                    case ZombieDetectType.ENEMY:
                        m_Brain.RefreshBrain(detectedEnemyState);
                        break;
                }
                
                isDetected = true;
            }
            
            
            return isDetected;
        }

        private void ResetSensor()
        {
            if (!isDetected) return;
            isDetected = false;

            switch (_currentDetectType)
            {
                case ZombieDetectType.RESOURCE:
                    m_Brain.GetBeliefStates().RemoveState(detectedResourceState);
                    break;
                case ZombieDetectType.ENEMY:
                    m_Brain.GetBeliefStates().RemoveState(detectedEnemyState);
                    break;
            }
            _currentDetectType = ZombieDetectType.NONE;
        }
    }

    public interface ITutorialRegister
    {
        public void Init();
    }
}