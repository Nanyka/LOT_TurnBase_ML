using System;
using System.Linq;
using GOAP;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

namespace JumpeeIsland
{
    public class MovementComp : MonoBehaviour, IMoveComp
    {
        [SerializeField] private float _stopDistance;

        private Transform m_Transform;
        private NavMeshPath _path;
        private Rigidbody _rigidbody;
        private Vector3 _currentDestination;
        private Vector3 _currentConner;
        private IProcessUpdate _currentProcessUpdate;
        private IMover _mover;
        private int _currentIndex;
        private bool _isLastPath;

        private void OnEnable()
        {
            _isLastPath = false;
        }

        private void OnDisable()
        {
            _isLastPath = false;
        }

        private void Start()
        {
            m_Transform = transform;
            _path = new NavMeshPath();
            _rigidbody = GetComponent<Rigidbody>();
            _mover = GetComponent<IMover>();
        }

        private void Update()
        {
            if (_isLastPath)
            {
                if (Vector3.Distance(m_Transform.position, _currentConner) < _stopDistance)
                {
                    _isLastPath = false;
                    StopMovement();
                }
            }
        }

        public void MoveTo(Vector3 destination, IProcessUpdate processUpdate)
        {
            _currentProcessUpdate = processUpdate;
            _currentConner = m_Transform.position;
            
            // Check if currentDestination and m_Transform.position is in NavMesh range
            // If not, select a nearest NavMesh point
            
            if (NavMesh.SamplePosition(destination, out NavMeshHit destinationHit, 2f, NavMesh.AllAreas))
            {
                if (destinationHit.hit)
                    _currentDestination = destinationHit.position;
                else
                    Debug.Log("MoveComp can't find a walkable destination");
            }
            
            if (NavMesh.SamplePosition(_currentConner, out NavMeshHit curPosHit, 2f, NavMesh.AllAreas))
            {
                if (curPosHit.hit)
                    _currentConner = curPosHit.position;
                else
                    Debug.Log("MoveComp can't find a walkable starting point");
            }
            
            // _mover = mover;
            _mover.StartWalk();
            StartMove();
        }

        private void StartMove()
        {
            // Check if entity close enough from destination
            // If not, calculate path
            // Move to the closet conner
            // Wait for entity reach the conner

            var currentPos = m_Transform.position;
            NavMesh.CalculatePath(currentPos, _currentDestination, NavMesh.AllAreas, _path);
            _path.corners.OrderBy(t => Vector3.Distance(currentPos, t));
            _currentIndex = 0;
            SetCurrentConner();
        }

        private void SetCurrentConner()
        {
            _currentIndex++;
            if (_currentIndex >= _path.corners.Length)
            {
                StopMovement();
                return;
            }

            // TODO: Check the distance by Update when it is the final corner
            _isLastPath = _currentIndex == _path.corners.Length - 1;

            RotateTowardCorner(_currentIndex);
        }

        private void StopMovement()
        {
            _currentProcessUpdate.StopProcess();
            _mover.StopWalk();
        }

        private void RotateTowardCorner(int cornerIndex)
        {
            var currentPos = m_Transform.position;
            _currentConner = _path.corners.ElementAt(cornerIndex);
            _currentConner = new Vector3(_currentConner.x, currentPos.y, _currentConner.z);

            // Rotate object to the conner
            Vector3 currentVelocity = _rigidbody.velocity;
            Quaternion offsetRotation = Quaternion.identity;
            if (currentVelocity != Vector3.zero)
                offsetRotation = Quaternion.FromToRotation(currentVelocity, Vector3.zero);
            m_Transform.rotation = Quaternion.LookRotation(_currentConner - currentPos) * offsetRotation;
        }

        // Walking animation move the object 1 meter and check the distance to the target at the end of the animation
        private void NewMovingLoop()
        {
            if (_isLastPath)
                return;

            if (Vector3.Distance(m_Transform.position,_currentConner) < _stopDistance)
                SetCurrentConner();
        }

        public float GetStopDistance()
        {
            return _stopDistance;
        }
    }

    public interface IMoveComp
    {
        public void MoveTo(Vector3 destination, IProcessUpdate processUpdate);
        public float GetStopDistance();
    }
}