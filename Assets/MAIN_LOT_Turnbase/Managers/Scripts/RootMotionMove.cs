using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class RootMotionMove : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        public List<Transform> tiles;

        private Transform m_Transform;
        private Vector3 direction;
        private Vector3 destination; // Ending point of the jump (Point B)
        private bool isStartMoves = false; // Flag to track if the object is in a move loop
        private bool isMoving = false; // Flag to track if the object is jumping
        private int moveIndex;
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int JumpUp = Animator.StringToHash("JumpUp");
        private static readonly int JumpDown = Animator.StringToHash("JumpDown");
        private static readonly int JumpUp1 = Animator.StringToHash("JumpUp1");
        private static readonly int JumpDown1 = Animator.StringToHash("JumpDown1");

        private void Start()
        {
            m_Transform = transform;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isStartMoves)
                ResetMoves();

            if (isStartMoves && !isMoving)
                StartMove();

            if (isMoving)
            {
                if (Vector3.Distance(destination, transform.position) < 0.1f)
                {
                    EndMove();
                }
            }
        }

        private void ResetMoves()
        {
            moveIndex = 0;
            isStartMoves = true;
        }

        private void StartMove()
        {
            destination = tiles[moveIndex].position;
            direction = new Vector3(destination.x, m_Transform.position.y, destination.z);
            m_Transform.LookAt(direction);

            if (Math.Abs(destination.y - transform.position.y) < 0.1f)
                m_Animator.SetBool(Walk, true);
            else if (destination.y > transform.position.y)
                m_Animator.SetTrigger(Mathf.Abs(Vector3.Distance(destination, transform.position)) < 1.5f
                    ? JumpUp1 : JumpUp);
            else if (destination.y < transform.position.y)
                m_Animator.SetTrigger(Mathf.Abs(Vector3.Distance(destination, transform.position)) < 1.5f
                    ? JumpDown1 : JumpDown);

            isMoving = true;
        }

        private void EndMove()
        {
            m_Animator.SetBool(Walk, false);
            var position = transform.position;
            position = new Vector3(Mathf.RoundToInt(position.x),
                Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
            transform.position = position;
            // Debug.Log($"End of move {moveIndex} at {transform.position}");
            
            isMoving = false;
            moveIndex++;

            if (moveIndex >= tiles.Count)
            {
                // Debug.Log($"End moves at {transform.position}");
                isStartMoves = false;
            }
        }
    }
}