using System;
using System.Linq;
using GOAP;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

namespace JumpeeIsland
{
    public class MovementComp : MonoBehaviour
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

        private void Start()
        {
            m_Transform = transform;
            _path = new NavMeshPath();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void MoveTo(Vector3 destination, IProcessUpdate processUpdate, IMover mover)
        {
            _currentDestination = destination;
            _currentProcessUpdate = processUpdate;
            _currentConner = m_Transform.position;
            _mover = mover;
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
                _currentProcessUpdate.StopProcess();
                _mover.StopWalk();
                return;
            }

            var currentPos = m_Transform.position;
            _currentConner = _path.corners.ElementAt(_currentIndex);
            _currentConner = new Vector3(_currentConner.x, currentPos.y, _currentConner.z);

            // Rotate object to the conner
            Vector3 currentVelocity = _rigidbody.velocity;
            Quaternion offsetRotation = Quaternion.identity;
            if (currentVelocity != Vector3.zero)
                offsetRotation = Quaternion.FromToRotation(currentVelocity, Vector3.zero);
            m_Transform.rotation = Quaternion.LookRotation(_currentConner - m_Transform.position) * offsetRotation;
        }

        // Walking animation move the object 1 meter and check the distance to the target at the end of the animation
        public void NewMovingLoop()
        {
            if (Vector3.Distance(m_Transform.position,
                    new Vector3(_currentConner.x, m_Transform.position.y, _currentConner.z)) < _stopDistance)
                SetCurrentConner();
        }

        public float GetStopDistance()
        {
            return _stopDistance;
        }
    }
}