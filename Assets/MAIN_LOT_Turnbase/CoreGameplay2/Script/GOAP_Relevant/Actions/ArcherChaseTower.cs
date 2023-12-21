using GOAP;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class ArcherChaseTower : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;

        private ICheckableObject _currentPoint;
        private float _distanceToAttack;
        private float _distanceToTower;
        private NavMeshPath _path;

        private void Start()
        {
            _distanceToAttack = m_GAgent.GetComponent<ISensor>().DetectRange();
            _path = new NavMeshPath();
        }

        public override bool PrePerform()
        {
            // Get the closet tower
            // Calculate the NavMesh path from the object to the tower
            // Check the length of the path
            // If the path is longer than distanceToAttack
            // Then, calculate a position along the path that is far from the tower a distanceToAttack miter
            
            CheckNearestTower();

            if (_currentPoint == null)
                return false;

            // Debug.Log($"Then nearest tower is at {_currentPoint.GetPosition()}");
            var myPos = transform.position;
            if (NavMesh.SamplePosition(myPos, out NavMeshHit curPosHit, 2f, NavMesh.AllAreas))
            {
                if (curPosHit.hit)
                    myPos = curPosHit.position;
            }
            
            NavMesh.CalculatePath(myPos, _currentPoint.GetPosition(), NavMesh.AllAreas, _path);

            // VisualDebug.Instance.DrawLine(_path.corners);

            float totalDistance = 0.0f;

            for (int i = 0; i < _path.corners.Length - 1; i++) {
                Vector3 currentWaypoint = _path.corners[i];
                Vector3 nextWaypoint = _path.corners[i + 1];
                float distance = Vector3.Distance(currentWaypoint, nextWaypoint);

                totalDistance += distance;
            }
            
            // Debug.Log($"Total distance: {totalDistance}");

            if (totalDistance > _distanceToAttack)
            {
                int closestWaypointIndex = _path.corners.Length - 1;
                float partialDistance = 0.0f;

                for (int i = 0; i < _path.corners.Length - 1; i++) {
                    partialDistance = 0.0f;

                    for (int j = i; j < _path.corners.Length - 1; j++) {
                        Vector3 currentWaypoint = _path.corners[j];
                        Vector3 nextWaypoint = _path.corners[j + 1];
                        float distance = Vector3.Distance(currentWaypoint, nextWaypoint);

                        partialDistance += distance;
                    }

                    // Debug.Log($"Partial distance at corner {i} is {partialDistance}");
                    if (partialDistance < _distanceToAttack)
                    {
                        closestWaypointIndex = i;
                        break;
                    }
                }

                var offset = 0f;
                if (partialDistance >= _distanceToAttack)
                    offset = _distanceToAttack / partialDistance;
                else
                    offset = (_distanceToAttack - partialDistance) / Vector3.Distance(_path.corners[closestWaypointIndex], _path.corners[closestWaypointIndex - 1]);
                // Debug.Log($"We are at corner {closestWaypointIndex} with partialDistance is {partialDistance}");
                // Debug.Log($"The offset ratio is {offset}");

                Vector3 interpolatedPoint = Vector3.Lerp(_path.corners[closestWaypointIndex], _path.corners[closestWaypointIndex - 1], offset);
                m_GAgent.SetIProcessUpdate(this, interpolatedPoint);
                
                // VisualDebug.Instance.ShowPointAt(interpolatedPoint);
                // Debug.Log($"Distance {totalDistance} compare to {_distanceToAttack}. Moving point: {interpolatedPoint}");
            }

            return true;
        }

        private void CheckNearestTower()
        {
            _currentPoint = null;
            var distanceToTarget = float.PositiveInfinity;
            var buildings = SavingSystemManager.Instance.GetEnvLoader().GetBuildings(FactionType.Enemy);

            foreach (var building in buildings)
            {
                if (building.TryGetComponent(out ICheckableObject checkableObject))
                {
                    if (checkableObject.IsCheckable() == false)
                        continue;

                    var curDis = Vector3.Distance(transform.position, checkableObject.GetPosition());
                    if (curDis < distanceToTarget)
                    {
                        distanceToTarget = curDis;
                        _currentPoint = checkableObject;
                    }
                }
            }
        }

        public override bool PostPerform()
        {
            return true;
        }

        public void StartProcess(Transform myTransform, Vector3 targetPos)
        {
            m_Entity.MoveTowards(targetPos, this);
        }

        public void StopProcess()
        {
            m_GAgent.FinishFromOutside();
        }
    }
}