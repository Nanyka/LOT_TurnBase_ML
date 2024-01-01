using System.Linq;
using GOAP;
using UnityEngine;
using UnityEngine.AI;

namespace JumpeeIsland
{
    public class ArcherChaseBoss : GAction, IProcessUpdate
    {
        [SerializeField] private CharacterEntity m_Entity;

        [SerializeField] private Vector3 _currentPoint;
        private float _distanceToTarget;
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
            // Get the boss position
            // Calculate the NavMesh path from the object to the boss
            // Check the length of the path
            // If the path is longer than distanceToAttack
            // Then, calculate a position along the path that is far from the tower a distanceToAttack miter

            CheckBossPosition();

            if (_distanceToTarget < float.PositiveInfinity)
            {
                // Debug.Log($"Then nearest tower is at {_currentPoint.GetPosition()}");
                var myPos = transform.position;
                if (NavMesh.SamplePosition(myPos, out NavMeshHit curPosHit, 2f, NavMesh.AllAreas))
                {
                    if (curPosHit.hit)
                        myPos = curPosHit.position;
                }

                NavMesh.CalculatePath(myPos, _currentPoint, NavMesh.AllAreas, _path);

                // VisualDebug.Instance.DrawLine(_path.corners);

                float totalDistance = 0.0f;

                for (int i = 0; i < _path.corners.Length - 1; i++)
                {
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

                    for (int i = 0; i < _path.corners.Length - 1; i++)
                    {
                        partialDistance = 0.0f;

                        for (int j = i; j < _path.corners.Length - 1; j++)
                        {
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
                        offset = (_distanceToAttack - partialDistance) /
                                 Vector3.Distance(_path.corners[closestWaypointIndex],
                                     _path.corners[closestWaypointIndex - 1]);
                    // Debug.Log($"We are at corner {closestWaypointIndex} with partialDistance is {partialDistance}");
                    // Debug.Log($"The offset ratio is {offset}");

                    Vector3 interpolatedPoint = Vector3.Lerp(_path.corners[closestWaypointIndex],
                        _path.corners[closestWaypointIndex - 1], offset);
                    m_GAgent.SetIProcessUpdate(this, interpolatedPoint);

                    // VisualDebug.Instance.ShowPointAt(interpolatedPoint);
                    // Debug.Log($"Distance {totalDistance} compare to {_distanceToAttack}. Moving point: {interpolatedPoint}");
                }

                return true;
            }
            else
                return false;
        }

        private void CheckBossPosition()
        {
            // _currentPoint = Vector3.positiveInfinity;
            _distanceToTarget = float.PositiveInfinity;
            var monsters = SavingSystemManager.Instance.GetMonsterController().GetMonsters();
            Debug.Log($"Archer check amount of boss: {monsters.Count()}");

            foreach (var monster in monsters)
            {
                var curDis = Vector3.Distance(transform.position, monster.transform.position);
                if (curDis < _distanceToTarget)
                {
                    _distanceToTarget = curDis;
                    _currentPoint = monster.transform.position;
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